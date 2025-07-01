document.getElementById("imageUpload").addEventListener("change", function () {
    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("imagePreview").src = e.target.result;
    };
    reader.readAsDataURL(this.files[0]);
});