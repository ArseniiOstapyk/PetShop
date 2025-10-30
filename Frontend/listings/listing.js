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
    document.getElementById("listing-description").textContent = listing.description;
    document.getElementById("listing-price").textContent = listing.price;
    document.getElementById("listing-type").textContent = listing.type;
    document.getElementById("listing-currency").textContent = listing.currency;
    document.getElementById("listing-photo").src =
      listing.photoUrl || "../assets/default-listing.jpg";

    const user = parseJwt(token);
    const role = user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    const userId = parseInt(
      user["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
    );

    const editBtn = document.getElementById("edit-btn");
    const deleteBtn = document.getElementById("delete-btn");
    const orderBtn = document.getElementById("order-btn");
    const actionSection = document.getElementById("action-buttons");
    const msg = document.getElementById("message");

    if (role === "Admin" || (role === "Seller" && listing.ownerId === userId)) {
      actionSection.classList.remove("hidden");
      editBtn.classList.remove("hidden");
      deleteBtn.classList.remove("hidden");

      editBtn.addEventListener("click", () => {
        window.location.href = `edit-listing.html?id=${id}`;
      });

      deleteBtn.addEventListener("click", () => handleDelete(id, token));
    }

    if (role === "User" || role === "Admin" || role === "Seller") {
      actionSection.classList.remove("hidden");
      orderBtn.classList.remove("hidden");

      orderBtn.addEventListener("click", async () => {
        if (!confirm("Add this listing to your order?")) return;
        msg.textContent = "Adding to order...";
        msg.style.color = "#444";

        try {
          const addRes = await fetch("http://localhost:5170/api/Orders/items", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ listingId: parseInt(id) }),
          });

          if (!addRes.ok) {
            const errText = await addRes.text();
            throw new Error(errText);
          }

          const data = await addRes.json();
          msg.textContent = "Listing added to your order!";
          msg.style.color = "green";

          setTimeout(() => {
            window.location.href = `../orders/order.html?id=${data.id}`;
          }, 1200);
        } catch (err) {
          console.error("Add to order error:", err);
          msg.textContent = "❌ " + err.message;
          msg.style.color = "red";
        }
      });
    }
  } catch (err) {
    document.body.innerHTML = `<h2>❌ ${err.message}</h2>`;
  }
});

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

async function handleDelete(id, token) {
  if (!confirm("Are you sure you want to delete this listing?")) return;

  const msg = document.getElementById("message");
  msg.textContent = "Deleting...";
  msg.style.color = "#444";

  try {
    const res = await fetch(`http://localhost:5170/api/Listings/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) throw new Error(await res.text());

    msg.textContent = "Listing deleted successfully!";
    msg.style.color = "green";

    setTimeout(() => {
      window.location.href = "listings.html";
    }, 1500);
  } catch (err) {
    msg.textContent = "❌ " + err.message;
    msg.style.color = "red";
  }
}