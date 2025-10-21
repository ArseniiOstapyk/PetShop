document.addEventListener("DOMContentLoaded", async () => {
  const tableBody = document.querySelector("#listings-table tbody");
  const addNewBtn = document.getElementById("add-new-btn");
  const typeFilter = document.getElementById("type-filter");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  // Decode user role from JWT
  const payload = parseJwt(token);
  const userRole = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (userRole === "Seller") {
    addNewBtn.classList.remove("hidden");
    addNewBtn.addEventListener("click", () => {
      window.location.href = "add-listing.html";
    });
  }

  let listings = [];

  try {
    // Load listings
    const res = await fetch("http://localhost:5170/api/Listings", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Failed to load listings");

    listings = await res.json();
    renderTable(listings);

    // Load listing types for filter
    await loadListingTypes();

    // Handle filter change
    typeFilter.addEventListener("change", () => {
      const selectedType = typeFilter.value;
      if (!selectedType) {
        renderTable(listings);
      } else {
        const filtered = listings.filter(l => l.type === selectedType);
        renderTable(filtered);
      }
    });
  } catch (err) {
    tableBody.innerHTML = `<tr><td colspan="5">❌ Error: ${err.message}</td></tr>`;
  }
});

// ✅ Decode JWT
function parseJwt(token) {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(jsonPayload);
  } catch {
    return {};
  }
}

// ✅ Render listings into table
function renderTable(data) {
  const tableBody = document.querySelector("#listings-table tbody");
  if (!data.length) {
    tableBody.innerHTML = `<tr><td colspan="5">No listings found</td></tr>`;
    return;
  }

  tableBody.innerHTML = data
    .map(
      (l) => `
    <tr onclick="window.location.href='listing.html?id=${l.id}'" class="clickable">
      <td><img src="${l.photoUrl || "../assets/default-listing.jpg"}" class="thumb"></td>
      <td>${l.name}</td>
      <td>${l.price.toFixed(2)}</td>
      <td>${l.currency}</td>
      <td>${l.type}</td>
    </tr>`
    )
    .join("");
}

// ✅ Load types from backend for dropdown
async function loadListingTypes() {
  const typeFilter = document.getElementById("type-filter");
  try {
    const res = await fetch("http://localhost:5170/api/Lookup/ListingTypes");
    if (!res.ok) throw new Error("Failed to load listing types");

    const types = await res.json();
    types.forEach((t) => {
      const option = document.createElement("option");
      option.value = t.name;
      option.textContent = t.name;
      typeFilter.appendChild(option);
    });
  } catch (err) {
    console.error("Listing type load error:", err);
  }
}