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
const role = payload?.role || payload?.Role || null;

const navLinks = document.getElementById("nav-links");

// ✅ Add seller tab
if (role === "Seller") {
  const li = document.createElement("li");
  li.innerHTML = `<a href="../dashboard/dashboard.html">Shop Dashboard</a>`;
  navLinks.appendChild(li);
}

// ✅ Add admin tab
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