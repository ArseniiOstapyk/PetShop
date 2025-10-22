document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const id = params.get("id");

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    window.location.href = "../login/login.html";
    return;
  }

  const payload = JSON.parse(atob(token.split(".")[1]));
  const role =
    payload.role ||
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (role !== "Admin") {
    document.body.innerHTML = "<h2>Access denied. Admins only.</h2>";
    return;
  }

  const userIdEl = document.getElementById("user-id");
  const userEmailEl = document.getElementById("user-email");
  const userPhoneEl = document.getElementById("user-phone");
  const userRoleEl = document.getElementById("user-role");
  const roleSelect = document.getElementById("role-select");
  const saveBtn = document.getElementById("save-btn");
  const cancelBtn = document.getElementById("cancel-btn");
  const messageBox = document.getElementById("message");

  try {
    const res = await fetch(`http://localhost:5170/api/Users/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("User not found");
    const user = await res.json();

    userIdEl.textContent = user.id;
    userEmailEl.textContent = user.email;
    userPhoneEl.textContent = user.phoneNumber || "-";
    userRoleEl.textContent = user.roleName || user.role || "-";

    const rolesRes = await fetch("http://localhost:5170/api/Lookup/Roles", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!rolesRes.ok) throw new Error("Failed to load roles");
    const roles = await rolesRes.json();

    roleSelect.innerHTML = roles
      .map(
        (r) =>
          `<option value="${r.id}" ${
            r.name === user.roleName ? "selected" : ""
          }>${r.name}</option>`
      )
      .join("");

   saveBtn.addEventListener("click", async () => {
  const newRoleName = roleSelect.options[roleSelect.selectedIndex].text;
  if (!newRoleName) {
    alert("Please select a role.");
    return;
  }

  messageBox.textContent = "Saving changes...";
  messageBox.style.color = "#444";

  try {
    const updateRes = await fetch(`http://localhost:5170/api/Users/${id}/role`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ role: newRoleName }),
    });

    if (!updateRes.ok) {
      const errText = await updateRes.text();
      throw new Error(errText);
    }

    messageBox.textContent = "User role updated successfully!";
    messageBox.style.color = "green";
    setTimeout(() => {
      window.location.href = "users.html";
    }, 1200);
  } catch (err) {
    console.error("Update error:", err);
    messageBox.textContent = "❌ " + err.message;
    messageBox.style.color = "red";
  }
});

    cancelBtn.addEventListener("click", () => {
      window.location.href = "users.html";
    });
  } catch (err) {
    console.error("Load user error:", err);
    document.body.innerHTML = `<h2>❌ ${err.message}</h2>`;
  }
});