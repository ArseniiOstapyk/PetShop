document.getElementById("login-form").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();
  const messageBox = document.getElementById("message");

  if (!email || !password) {
    messageBox.style.color = "red";
    messageBox.textContent = "Please enter both email and password.";
    return;
  }

  messageBox.style.color = "#444";
  messageBox.textContent = "🔄 Logging in...";

  try {
    const response = await fetch("http://localhost:5170/api/Auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
      let errorMsg = "Login failed. Please check your credentials.";
      try {
        const errorData = await response.json();
        if (errorData?.message) errorMsg = errorData.message;
      } catch {}
      throw new Error(errorMsg);
    }

    const data = await response.json();

    if (!data.token) throw new Error("No token received from server.");

    localStorage.setItem("jwtToken", data.token);

    messageBox.style.color = "green";
    messageBox.textContent = "Login successful! Redirecting...";
    setTimeout(() => {
      window.location.href = "../home/home.html";
    }, 1500);

  } catch (error) {
    console.error("Login error:", error);
    messageBox.style.color = "red";
    messageBox.textContent = "❌ " + error.message;
  }
});

document.getElementById("forgot-link").addEventListener("click", (e) => {
  e.preventDefault();
  window.location.href = "../forgot-password/forgot.html";
});

document.getElementById("register-link").addEventListener("click", (e) => {
  e.preventDefault();
  window.location.href = "../register/register.html";
});