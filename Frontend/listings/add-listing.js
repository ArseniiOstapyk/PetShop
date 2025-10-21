document.addEventListener("DOMContentLoaded", async () => {
  await loadCurrencies();
  await loadListingTypes();
});

// ✅ Load currencies
async function loadCurrencies() {
  const currencySelect = document.getElementById("currency");
  try {
    const res = await fetch("http://localhost:5170/api/Lookup/Currencies");
    if (!res.ok) throw new Error("Failed to load currencies");
    const data = await res.json();

    currencySelect.innerHTML = "<option value=''>-- Select Currency --</option>";
    data.forEach(c => {
      const option = document.createElement("option");
      option.value = c.id; // must be numeric
      option.textContent = `${c.name} (${c.symbol})`;
      currencySelect.appendChild(option);
    });

    console.log("✅ Loaded currencies:", data);
  } catch (err) {
    console.error("❌ Currency load error:", err);
    currencySelect.innerHTML = "<option>Error loading currencies</option>";
  }
}

// ✅ Load listing types
async function loadListingTypes() {
  const typeSelect = document.getElementById("type");
  try {
    const res = await fetch("http://localhost:5170/api/Lookup/ListingTypes");
    if (!res.ok) throw new Error("Failed to load listing types");
    const data = await res.json();

    typeSelect.innerHTML = "<option value=''>-- Select Type --</option>";
    data.forEach(t => {
      const option = document.createElement("option");
      option.value = t.id; // must be numeric
      option.textContent = t.name;
      typeSelect.appendChild(option);
    });

    console.log("✅ Loaded listing types:", data);
  } catch (err) {
    console.error("❌ Listing types load error:", err);
    typeSelect.innerHTML = "<option>Error loading types</option>";
  }
}

// ✅ Handle form submission
document.getElementById("listing-form").addEventListener("submit", async (e) => {
  e.preventDefault();

  const messageBox = document.getElementById("message");
  messageBox.textContent = "🔄 Uploading listing...";
  messageBox.style.color = "#444";

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    messageBox.style.color = "red";
    messageBox.textContent = "❌ You must be logged in to add a listing.";
    return;
  }

  try {
    // 1️⃣ Upload photo (if any)
    let photoId = null;
    const photoFile = document.getElementById("photo").files[0];
    if (photoFile) {
      const photoForm = new FormData();
      photoForm.append("file", photoFile);

      const uploadRes = await fetch("http://localhost:5170/api/Photos/upload", {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
        body: photoForm
      });

      if (!uploadRes.ok) {
        const errText = await uploadRes.text();
        throw new Error("Photo upload failed: " + errText);
      }

      const photoData = await uploadRes.json();
      photoId = photoData.id;
      console.log("📸 Uploaded photo with ID:", photoId);
    }

    // 2️⃣ Validate dropdowns and fields
    const currencySelect = document.getElementById("currency");
    const typeSelect = document.getElementById("type");
    const priceValue = parseFloat(document.getElementById("price").value);

    const currencyValue = Number(currencySelect.value);
    const typeValue = Number(typeSelect.value);

    console.log("💡 Selected:", { currencyValue, typeValue });

    if (!currencyValue || !typeValue) {
      messageBox.style.color = "red";
      messageBox.textContent = "❌ Please select both currency and type.";
      return;
    }

    if (isNaN(priceValue) || priceValue <= 0) {
      messageBox.style.color = "red";
      messageBox.textContent = "❌ Please enter a valid price.";
      return;
    }

    // 3️⃣ Build listing payload
    const listingData = {
      name: document.getElementById("name").value.trim(),
      description: document.getElementById("description").value.trim(),
      price: priceValue,
      currencyId: currencyValue,
      typeId: typeValue,
      photoId: photoId,
      isAvailable: true
    };

    console.log("📦 Sending listing data:", listingData);

    // 4️⃣ Send to backend
    const res = await fetch("http://localhost:5170/api/Listings", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(listingData)
    });

    if (!res.ok) {
      const errText = await res.text();
      throw new Error(errText);
    }

    messageBox.style.color = "green";
    messageBox.textContent = "✅ Listing added successfully!";
    setTimeout(() => window.location.href = "listings.html", 1500);

  } catch (err) {
    console.error("❌ Add listing error:", err);
    messageBox.style.color = "red";
    messageBox.textContent = "❌ " + err.message;
  }
});