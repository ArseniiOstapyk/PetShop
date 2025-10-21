document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const id = params.get("id");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  try {
    const res = await fetch(`http://localhost:5170/api/Listings/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Listing not found");

    const listing = await res.json();

    document.getElementById("listing-name").textContent = listing.name;
    document.getElementById("listing-description").textContent =
      listing.description;
    document.getElementById("listing-price").textContent = listing.price;
    document.getElementById("listing-type").textContent = listing.type;
    document.getElementById("listing-currency").textContent = listing.currency;
    document.getElementById("listing-photo").src =
      listing.photoUrl || "../assets/default-listing.jpg";
  } catch (err) {
    document.body.innerHTML = `<h2>❌ ${err.message}</h2>`;
  }
});