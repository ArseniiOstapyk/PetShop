document.addEventListener("DOMContentLoaded", async () => {
  const tableBody = document.querySelector("#listings-table tbody");
  const addNewBtn = document.getElementById("add-new-btn");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  // ✅ Decode role from JWT (client-side)
  const payload = JSON.parse(atob(token.split(".")[1]));
  const userRole = payload.role;

  if (userRole === "Seller") {
    addNewBtn.classList.remove("hidden");
    addNewBtn.addEventListener("click", () => {
      window.location.href = "add-listing.html";
    });
  }

  try {
    const res = await fetch("http://localhost:5170/api/Listings", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Failed to load listings");

    const listings = await res.json();
    tableBody.innerHTML = listings
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
  } catch (err) {
    tableBody.innerHTML = `<tr><td colspan="5">❌ Error: ${err.message}</td></tr>`;
  }
});