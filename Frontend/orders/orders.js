document.addEventListener("DOMContentLoaded", async () => {
  const tableBody = document.querySelector("#orders-table tbody");
  const messageBox = document.getElementById("message");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  try {
    const res = await fetch("http://localhost:5170/api/Orders", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) throw new Error("Failed to load orders");

    const orders = await res.json();

    if (!orders.length) {
      tableBody.innerHTML = "<tr><td colspan='3'>No orders found</td></tr>";
      return;
    }

    renderTable(orders);
  } catch (err) {
    console.error("❌ Orders load error:", err);
    messageBox.textContent = "❌ " + err.message;
  }
});

function renderTable(data) {
  const tableBody = document.querySelector("#orders-table tbody");

  tableBody.innerHTML = data
    .map(
      (o) => `
      <tr onclick="window.location.href='order.html?id=${o.id}'" class="clickable">
        <td>${o.number}</td>
        <td>${o.isPaid ? "✅ Yes" : "❌ No"}</td>
        <td>${o.listingsCount}</td>
      </tr>`
    )
    .join("");
}