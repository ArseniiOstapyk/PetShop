document.getElementById("forgot-form").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("email").value.trim();
  const messageBox = document.getElementById("message");

  if (!email) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ Please enter your email.";
    return;
  }

  messageBox.style.color = "#444";
  messageBox.textContent = "🔄 Sending reset link...";

  try {
    const response = await fetch("http://localhost:5170/api/Auth/forgot-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email }),
    });

    if (!response.ok) {
      let errorMsg = "Failed to send reset link.";
      try {
        const errData = await response.json();
        if (errData?.message) errorMsg = errData.message;
      } catch {}
      throw new Error(errorMsg);
    }

    // ✅ Success
    messageBox.style.color = "green";
    messageBox.textContent = "✅ If this email exists, a reset link has been sent.";
  } catch (error) {
    console.error("Forgot password error:", error);
    messageBox.style.color = "red";
    messageBox.textContent = "❌ " + error.message;
  }
});