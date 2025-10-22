document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const id = params.get("id");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  const payload = JSON.parse(atob(token.split(".")[1]));
  const currentUserId =
    parseInt(payload.nameid) ||
    parseInt(payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);

  const numberEl = document.getElementById("order-number");
  const dateEl = document.getElementById("order-date");
  const paidEl = document.getElementById("order-paid");
  const itemsEl = document.getElementById("order-items");
  const sumEl = document.getElementById("order-sum");
  const tableBody = document.querySelector("#items-table tbody");
  const messageBox = document.getElementById("message");

  const markPaidBtn = document.getElementById("mark-paid-btn");
  const removeSelectedBtn = document.getElementById("remove-selected-btn");
  const deleteBtn = document.getElementById("delete-btn");
  const selectAll = document.getElementById("select-all");

  try {
    const res = await fetch(`http://localhost:5170/api/Orders/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Order not found");

    const order = await res.json();

    numberEl.textContent = order.number;
    dateEl.textContent = new Date(order.createdOn).toLocaleString();
    paidEl.textContent = order.isPaid ? "✅ Yes" : "❌ No";
    itemsEl.textContent = order.listingsCount;
    sumEl.textContent = order.totalSum.toFixed(2);

    tableBody.innerHTML = order.items
      .map(
        (i) => `
        <tr>
          <td><input type="checkbox" class="item-checkbox" data-id="${i.orderListingId}" /></td>
          <td><a href="../listings/listing.html?id=${i.listingId}">${i.listingName}</a></td>
          <td>${i.price.toFixed(2)}</td>
          <td>${i.sellerEmail || "-"}</td>
        </tr>`
      )
      .join("");

    markPaidBtn.classList.toggle("hidden", order.isPaid);
    removeSelectedBtn.classList.remove("hidden");
    deleteBtn.classList.remove("hidden");

    selectAll.addEventListener("change", () => {
      document
        .querySelectorAll(".item-checkbox")
        .forEach((cb) => (cb.checked = selectAll.checked));
      toggleRemoveButton();
    });

    document.addEventListener("change", (e) => {
      if (e.target.classList.contains("item-checkbox")) toggleRemoveButton();
    });

    function toggleRemoveButton() {
      const anyChecked = document.querySelectorAll(".item-checkbox:checked").length > 0;
      removeSelectedBtn.disabled = !anyChecked;
      removeSelectedBtn.style.opacity = anyChecked ? "1" : "0.6";
      removeSelectedBtn.style.cursor = anyChecked ? "pointer" : "not-allowed";
    }
    toggleRemoveButton();

    removeSelectedBtn.addEventListener("click", async () => {
      const selectedIds = Array.from(document.querySelectorAll(".item-checkbox:checked")).map(
        (cb) => cb.dataset.id
      );
      if (selectedIds.length === 0) return;

      if (!confirm(`Remove ${selectedIds.length} selected item(s)?`)) return;

      messageBox.textContent = "Removing selected items...";
      messageBox.style.color = "#444";

      try {
        for (const orderListingId of selectedIds) {
          const delRes = await fetch(
            `http://localhost:5170/api/Orders/${id}/items/${orderListingId}`,
            {
              method: "DELETE",
              headers: { Authorization: `Bearer ${token}` },
            }
          );
          if (!delRes.ok) throw new Error(await delRes.text());
        }

        messageBox.textContent = "Selected items removed!";
        messageBox.style.color = "green";
        setTimeout(() => window.location.reload(), 800);
      } catch (err) {
        messageBox.textContent = "❌ " + err.message;
        messageBox.style.color = "red";
      }
    });

    markPaidBtn.addEventListener("click", async () => {
      if (!confirm("Mark this order as paid?")) return;

      const paidRes = await fetch(`http://localhost:5170/api/Orders/${id}/pay`, {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!paidRes.ok) throw new Error(await paidRes.text());

      alert("Order marked as paid!");
      window.location.reload();
    });

    deleteBtn.addEventListener("click", async () => {
      if (!confirm("Are you sure you want to delete this order?")) return;

      const delRes = await fetch(`http://localhost:5170/api/Orders/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!delRes.ok) throw new Error(await delRes.text());

      alert("🗑️ Order deleted successfully!");
      window.location.href = "orders.html";
    });
  } catch (err) {
    console.error("Order load error:", err);
    messageBox.textContent = "❌ " + err.message;
  }
});