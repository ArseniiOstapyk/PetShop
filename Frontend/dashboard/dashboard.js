document.addEventListener("DOMContentLoaded", async () => {
  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  // Decode role
  const payload = JSON.parse(atob(token.split(".")[1]));
  const role = payload.role || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (role !== "Seller") {
    document.body.innerHTML = "<h2>❌ Access denied. Sellers only.</h2>";
    return;
  }

  const tableBody = document.querySelector("#listings-table tbody");
  const typeFilter = document.getElementById("type-filter");
  const totalListingsEl = document.getElementById("total-listings");
  const totalSumEl = document.getElementById("total-sum");
  const addNewBtn = document.getElementById("add-new-btn");
  const messageBox = document.getElementById("message");

  // Load listing types
  try {
    const typesRes = await fetch("http://localhost:5170/api/Lookup/ListingTypes", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const types = await typesRes.json();
    typeFilter.innerHTML += types.map((t) => `<option value="${t.name}">${t.name}</option>`).join("");
  } catch (err) {
    console.error("Failed to load listing types:", err);
  }

  // Load listings of the current seller
  let listings = [];
  async function loadListings() {
    messageBox.textContent = "Loading...";
    messageBox.style.color = "#444";

    try {
      const res = await fetch("http://localhost:5170/api/Listings", {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error("Failed to load listings");

      const allListings = await res.json();
      // Filter only seller’s own listings
      const userId = parseInt(
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
      );
      listings = allListings.filter((l) => l.ownerId === userId);

      renderTable(listings);
      updateDashboard(listings);

      messageBox.textContent = "";
    } catch (err) {
      console.error(err);
      messageBox.textContent = "❌ " + err.message;
      messageBox.style.color = "red";
    }
  }

  // Render listings table
  function renderTable(data) {
    if (data.length === 0) {
      tableBody.innerHTML = `<tr><td colspan="5">No listings found.</td></tr>`;
      return;
    }

    tableBody.innerHTML = data
      .map(
        (l) => `
        <tr onclick="window.location.href='../listings/listing.html?id=${l.id}'" class="clickable">
          <td><img src="${l.photoUrl || "../assets/default-listing.jpg"}" class="thumb"></td>
          <td>${l.name}</td>
          <td>${l.price.toFixed(2)}</td>
          <td>${l.currency}</td>
          <td>${l.type}</td>
        </tr>`
      )
      .join("");
  }

  // Dashboard summary
  function updateDashboard(list) {
    totalListingsEl.textContent = list.length;
    const total = list.reduce((sum, l) => sum + l.price, 0);
    totalSumEl.textContent = "$" + total.toFixed(2);
  }

  // Filtering by Type
  typeFilter.addEventListener("change", () => {
    const selectedType = typeFilter.value;
    const filtered =
      selectedType === "" ? listings : listings.filter((l) => l.type === selectedType);
    renderTable(filtered);
    updateDashboard(filtered);
  });

  // Add new listing
  addNewBtn.addEventListener("click", () => {
    window.location.href = "../listings/add-listing.html";
  });

  // Initial load
  await loadListings();
});