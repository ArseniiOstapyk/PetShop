document.addEventListener("DOMContentLoaded", async () => {
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

  const tableBody = document.querySelector("#users-table tbody");
  const removeSelectedBtn = document.getElementById("remove-selected-btn");
  const selectAll = document.getElementById("select-all");
  const messageBox = document.getElementById("message");
  const roleFilter = document.getElementById("role-filter");

  let allUsers = [];

  try {
    const rolesRes = await fetch("http://localhost:5170/api/Lookup/Roles", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!rolesRes.ok) throw new Error("Failed to load roles list.");

    const roles = await rolesRes.json();
    roleFilter.innerHTML =
      `<option value="">All Roles</option>` +
      roles.map((r) => `<option value="${r.name}">${r.name}</option>`).join("");

    const res = await fetch("http://localhost:5170/api/Users", {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Failed to load users.");

    allUsers = await res.json();
    renderUsers(allUsers);

    roleFilter.addEventListener("change", () => {
      const selectedRole = roleFilter.value;
      const filtered =
        selectedRole === ""
          ? allUsers
          : allUsers.filter(
              (u) =>
                u.roleName?.toLowerCase() === selectedRole.toLowerCase() ||
                u.role?.toLowerCase() === selectedRole.toLowerCase()
            );
      renderUsers(filtered);
    });
  } catch (err) {
    console.error("Load users error:", err);
    messageBox.textContent = "❌ " + err.message;
  }

  function renderUsers(users) {
    if (!users.length) {
      tableBody.innerHTML = `<tr><td colspan="7">No users found.</td></tr>`;
      return;
    }

    tableBody.innerHTML = users
      .map(
        (u) => `
        <tr class="clickable" data-id="${u.id}">
          <td><input type="checkbox" class="user-checkbox" data-id="${u.id}" /></td>
          <td>${u.id}</td>
          <td>${u.email}</td>
          <td>${u.phoneNumber || "-"}</td>
          <td>${u.roleName || u.role || "-"}</td>
          <td>${new Date(u.createdOn).toLocaleDateString()}</td>
          <td><button class="action-delete" data-id="${u.id}">🗑️ Delete</button></td>
        </tr>`
      )
      .join("");

    attachEventHandlers();
  }

  function attachEventHandlers() {
    document.querySelectorAll("#users-table tbody tr").forEach((row) => {
      row.addEventListener("click", (e) => {
        if (
          e.target.tagName === "INPUT" ||
          e.target.classList.contains("action-delete")
        )
          return;
        const id = row.dataset.id;
        window.location.href = `user-edit.html?id=${id}`;
      });
    });

    document.querySelectorAll(".action-delete").forEach((btn) =>
      btn.addEventListener("click", async (e) => {
        e.stopPropagation();
        const userId = e.target.dataset.id;
        if (!confirm("Delete this user?")) return;
        await deleteUser(userId);
      })
    );

    selectAll.addEventListener("change", () => {
      document
        .querySelectorAll(".user-checkbox")
        .forEach((cb) => (cb.checked = selectAll.checked));
      toggleRemoveButton();
    });

    document.addEventListener("change", (e) => {
      if (e.target.classList.contains("user-checkbox")) toggleRemoveButton();
    });

    removeSelectedBtn.addEventListener("click", async () => {
      const selectedIds = Array.from(
        document.querySelectorAll(".user-checkbox:checked")
      ).map((cb) => cb.dataset.id);

      if (selectedIds.length === 0) {
        alert("Please select at least one user to remove.");
        return;
      }

      if (!confirm(`Remove ${selectedIds.length} selected user(s)?`)) return;

      messageBox.textContent = "Removing selected users...";
      messageBox.style.color = "#444";

      for (const id of selectedIds) {
        await deleteUser(id, false);
      }

      messageBox.textContent = "✅ Selected users removed!";
      messageBox.style.color = "green";
      setTimeout(() => window.location.reload(), 1000);
    });
  }

  function toggleRemoveButton() {
    const anyChecked =
      document.querySelectorAll(".user-checkbox:checked").length > 0;
    removeSelectedBtn.disabled = !anyChecked;
    removeSelectedBtn.style.opacity = anyChecked ? "1" : "0.6";
    removeSelectedBtn.style.cursor = anyChecked ? "pointer" : "not-allowed";
  }

  async function deleteUser(id, reload = true) {
    try {
      const delRes = await fetch(`http://localhost:5170/api/Users/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!delRes.ok) throw new Error(await delRes.text());

      if (reload) {
        alert("User deleted successfully!");
        window.location.reload();
      }
    } catch (err) {
      console.error("Delete user error:", err);
      messageBox.textContent = "❌ " + err.message;
      messageBox.style.color = "red";
    }
  }
});