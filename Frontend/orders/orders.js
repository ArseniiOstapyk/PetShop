document.addEventListener("DOMContentLoaded", async () => {
  const tableBody = document.querySelector("#orders-table tbody");
  const messageBox = document.getElementById("message");
  const filterSelect = document.getElementById("paid-filter");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  let allOrders = [];

  try {
    const res = await fetch("http://localhost:5170/api/Orders", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) throw new Error("Failed to load orders");

    allOrders = await res.json();
    renderTable(allOrders);
  } catch (err) {
    console.error("Orders load error:", err);
    messageBox.textContent = "❌ " + err.message;
  }

  filterSelect.addEventListener("change", () => {
    const value = filterSelect.value;
    let filtered = allOrders;

    if (value === "paid") {
      filtered = allOrders.filter(o => o.isPaid);
    } else if (value === "unpaid") {
      filtered = allOrders.filter(o => !o.isPaid);
    }

    renderTable(filtered);
  });
});

function renderTable(data) {
  const tableBody = document.querySelector("#orders-table tbody");

  if (!data.length) {
    tableBody.innerHTML = "<tr><td colspan='4'>No orders found</td></tr>";
    return;
  }

  tableBody.innerHTML = data
    .map(
      (o) => `
      <tr onclick="window.location.href='order.html?id=${o.id}'" class="clickable">
        <td>${o.number}</td>
        <td>${o.isPaid ? "✅ Yes" : "❌ No"}</td>
        <td>${o.listingsCount}</td>
        <td>${o.totalSum.toFixed(2)}</td>
      </tr>`
    )
    .join("");
}