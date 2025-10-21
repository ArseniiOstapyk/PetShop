document.addEventListener("DOMContentLoaded", () => {
  const urlParams = new URLSearchParams(window.location.search);
  const token = urlParams.get("token");
  const messageBox = document.getElementById("message");

  if (!token) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ Invalid or missing reset token.";
    document.getElementById("reset-form").style.display = "none";
    return;
  }

  document.getElementById("reset-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    const password = document.getElementById("password").value.trim();
    const confirmPassword = document.getElementById("confirm-password").value.trim();

    if (password.length < 6) {
      messageBox.style.color = "red";
      messageBox.textContent = "❌ Password must be at least 6 characters.";
      return;
    }

    if (password !== confirmPassword) {
      messageBox.style.color = "red";
      messageBox.textContent = "❌ Passwords do not match.";
      return;
    }

    messageBox.style.color = "#444";
    messageBox.textContent = "🔄 Resetting password...";

    try {
      const response = await fetch("http://localhost:5170/api/Auth/reset-password", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ token, newPassword: password }),
      });

      if (!response.ok) {
        let errorMsg = "Password reset failed.";
        try {
          const errorData = await response.json();
          if (errorData?.message) errorMsg = errorData.message;
        } catch {}
        throw new Error(errorMsg);
      }

      messageBox.style.color = "green";
      messageBox.textContent = "✅ Password reset successfully! Redirecting to login...";
      setTimeout(() => (window.location.href = "../login/login.html"), 2500);

    } catch (error) {
      console.error("Reset error:", error);
      messageBox.style.color = "red";
      messageBox.textContent = "❌ " + error.message;
    }
  });
});