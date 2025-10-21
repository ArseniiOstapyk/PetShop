document.getElementById("register-form").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("email").value.trim();
  const phone = document.getElementById("phone").value.trim();
  const password = document.getElementById("password").value.trim();
  const confirmPassword = document.getElementById("confirm-password").value.trim();
  const messageBox = document.getElementById("message");

  // ✅ Simple frontend validation
  if (!email || !password || !confirmPassword) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ Please fill out all required fields.";
    return;
  }

  if (password !== confirmPassword) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ Passwords do not match.";
    return;
  }

  messageBox.style.color = "#333";
  messageBox.textContent = "🔄 Registering...";

  // ✅ Build request body dynamically
  const requestBody = { email, password };
  if (phone) requestBody.phoneNumber = phone; // only include if provided

  try {
    const response = await fetch("http://localhost:5170/api/Auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(requestBody),
    });

    if (!response.ok) {
      let errorMsg = "Registration failed.";
      try {
        const errorData = await response.json();
        if (errorData?.message) errorMsg = errorData.message;
      } catch {}
      throw new Error(errorMsg);
    }

    // ✅ Success
    messageBox.style.color = "green";
    messageBox.textContent = "✅ Registration successful! Redirecting to login...";
    setTimeout(() => (window.location.href = "../login/login.html"), 1500);

  } catch (error) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ " + error.message;
  }
});