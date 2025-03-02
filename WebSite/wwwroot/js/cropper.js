let cropper;

document.getElementById('fileInput').addEventListener('change', function (e) {
    const files = e.target.files;
    const reader = new FileReader();

    reader.onload = function (event) {
        const image = document.getElementById('image');
        image.src = event.target.result;

        if (cropper) {
            cropper.destroy();
        }
        cropper = new Cropper(image, {
            aspectRatio: 3 / 4,
            viewMode: 1,
            autoCropArea: 0.8,
            responsive: true,
            zoomable: true,
            scalable: true,
            cropBoxResizable: true,
        });
    };
    reader.readAsDataURL(files[0]);
});

document.getElementById('productForm').addEventListener('submit', function (event) {
    event.preventDefault();
    if (cropper) {
        const croppedImage = cropper.getCroppedCanvas().toDataURL('image/png');
        document.getElementById('productImage').value = croppedImage;
    }
    this.submit();
});