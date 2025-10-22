const token = localStorage.getItem("jwtToken");
if (!token) {
  window.location.href = "../login/login.html";
}

// ✅ Decode JWT payload to read the role
function parseJwt(token) {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    return null;
  }
}

const payload = parseJwt(token);

// ✅ Support all possible role claim formats
const role =
  payload?.role ||
  payload?.Role ||
  payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
  payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"] ||
  null;

const navLinks = document.getElementById("nav-links");

// ✅ Add Seller tab
if (role === "Seller") {
  const li = document.createElement("li");
  li.innerHTML = `<a href="../dashboard/dashboard.html">Shop Dashboard</a>`;
  navLinks.appendChild(li);
}

// ✅ Add Admin tab (Users management)
if (role === "Admin") {
  const li = document.createElement("li");
  li.innerHTML = `<a href="../users/users.html">Users</a>`;
  navLinks.appendChild(li);
}

// ✅ Logout button
document.getElementById("logout-btn").addEventListener("click", () => {
  localStorage.removeItem("jwtToken");
  window.location.href = "../login/login.html";
});