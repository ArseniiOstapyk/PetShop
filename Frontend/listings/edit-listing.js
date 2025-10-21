document.addEventListener("DOMContentLoaded", async () => {
  const id = new URLSearchParams(window.location.search).get("id");
  if (!id) {
    alert("Missing listing ID");
    window.location.href = "listings.html";
    return;
  }

  // Load everything
  await loadListingData(id);
  document.getElementById("listing-form").addEventListener("submit", (e) => handleSubmit(e, id));
  document.getElementById("photo").addEventListener("change", previewPhoto);
});

// ✅ Preview selected photo
function previewPhoto(e) {
  const file = e.target.files[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = (ev) => {
      document.getElementById("photo-preview").src = ev.target.result;
    };
    reader.readAsDataURL(file);
  }
}

// ✅ Load currencies
async function loadCurrencies(selectedId = null, selectedName = null) {
  const select = document.getElementById("currency");
  const res = await fetch("http://localhost:5170/api/Lookup/Currencies");
  const data = await res.json();
  select.innerHTML = "<option value=''>-- Select Currency --</option>";
  data.forEach(c => {
    const opt = document.createElement("option");
    opt.value = c.id;
    opt.textContent = `${c.name} (${c.symbol})`;
    select.appendChild(opt);
  });

  // Try matching by ID first, then by name
  if (selectedId) select.value = selectedId;
  else if (selectedName) {
    const match = Array.from(select.options).find(o => o.text.includes(selectedName));
    if (match) match.selected = true;
  }
}

// ✅ Load listing types
async function loadListingTypes(selectedId = null, selectedName = null) {
  const select = document.getElementById("type");
  const res = await fetch("http://localhost:5170/api/Lookup/ListingTypes");
  const data = await res.json();
  select.innerHTML = "<option value=''>-- Select Type --</option>";
  data.forEach(t => {
    const opt = document.createElement("option");
    opt.value = t.id;
    opt.textContent = t.name;
    select.appendChild(opt);
  });

  // Try matching by ID first, then by name
  if (selectedId) select.value = selectedId;
  else if (selectedName) {
    const match = Array.from(select.options).find(o => o.text === selectedName);
    if (match) match.selected = true;
  }
}

// ✅ Load listing data
async function loadListingData(id) {
  const token = localStorage.getItem("jwtToken");
  const res = await fetch(`http://localhost:5170/api/Listings/${id}`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!res.ok) {
    alert("Failed to load listing data.");
    window.location.href = "listings.html";
    return;
  }

  const data = await res.json();

  // Fill text fields
  document.getElementById("name").value = data.name || "";
  document.getElementById("description").value = data.description || "";
  document.getElementById("price").value = data.price || "";
  document.getElementById("isAvailable").checked = data.isAvailable || false;

  // Load dropdowns AFTER we know what to select
  await loadCurrencies(data.currencyId, data.currency);
  await loadListingTypes(data.typeId, data.type);

  // Set photo
  const photoPreview = document.getElementById("photo-preview");
  if (data.photoUrl) photoPreview.src = data.photoUrl;
  else photoPreview.src = "../assets/default-listing.jpg";
}

// ✅ Submit PUT update
async function handleSubmit(e, id) {
  e.preventDefault();
  const msg = document.getElementById("message");
  msg.textContent = "Saving...";
  msg.style.color = "#444";

  const token = localStorage.getItem("jwtToken");

  try {
    // Upload new photo if chosen
    let photoId = null;
    const photo = document.getElementById("photo").files[0];
    if (photo) {
      const fd = new FormData();
      fd.append("file", photo);
      const upload = await fetch("http://localhost:5170/api/Photos/upload", {
        method: "POST",
        headers: { Authorization: `Bearer ${token}` },
        body: fd
      });
      const result = await upload.json();
      photoId = result.id;
    }

    const payload = {
      name: document.getElementById("name").value.trim(),
      description: document.getElementById("description").value.trim(),
      price: parseFloat(document.getElementById("price").value),
      currencyId: parseInt(document.getElementById("currency").value),
      typeId: parseInt(document.getElementById("type").value),
      photoId: photoId,
      isAvailable: document.getElementById("isAvailable").checked
    };

    const res = await fetch(`http://localhost:5170/api/Listings/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    });

    if (!res.ok) throw new Error(await res.text());

    msg.textContent = "✅ Listing updated successfully!";
    msg.style.color = "green";
    setTimeout(() => (window.location.href = `listing.html?id=${id}`), 1500);
  } catch (err) {
    console.error(err);
    msg.textContent = "❌ " + err.message;
    msg.style.color = "red";
  }
}