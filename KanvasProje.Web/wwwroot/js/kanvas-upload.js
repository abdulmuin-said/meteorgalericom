(() => {
    const form = document.getElementById("kanvasUploadForm");
    const dropZone = document.getElementById("dropZone");
    const fileInput = document.getElementById("zipDosyasi");
    const fileInfo = document.getElementById("fileInfo");
    const selectedFileName = document.getElementById("selectedFileName");
    const selectedFileSize = document.getElementById("selectedFileSize");
    const btnChangeFile = document.getElementById("btnChangeFile");
    const progress = document.getElementById("uploadProgress");
    const progressBar = document.getElementById("uploadProgressBar");
    const status = document.getElementById("uploadStatus");
    const button = document.getElementById("uploadButton");

    if (!form || !button || !fileInput) return;

    const chunkSize = 8 * 1024 * 1024; // 8 MB
    const maxFileSize = 300 * 1024 * 1024; // 300 MB
    let selectedFile = null;

    function formatBytes(bytes) {
        if (!bytes || bytes === 0) return "0 Bytes";
        const k = 1024;
        const dm = 1;
        const sizes = ["Bytes", "KB", "MB", "GB"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
    }

    function setFile(file) {
        status.textContent = "";
        status.className = "mt-3 fw-semibold small";

        if (!file) return;

        if (!file.name.toLowerCase().endsWith(".zip")) {
            status.className = "mt-3 fw-semibold small text-danger";
            status.textContent = "Lütfen sadece .zip formatında bir arşiv dosyası seçin.";
            fileInput.value = "";
            selectedFile = null;
            if (fileInfo) fileInfo.classList.add("d-none");
            return;
        }

        if (file.size > maxFileSize) {
            status.className = "mt-3 fw-semibold small text-danger";
            status.textContent = `Dosya boyutu çok büyük (${formatBytes(file.size)}). En fazla 300 MB yüklenebilir.`;
            fileInput.value = "";
            selectedFile = null;
            if (fileInfo) fileInfo.classList.add("d-none");
            return;
        }

        selectedFile = file;
        if (selectedFileName) selectedFileName.textContent = file.name;
        if (selectedFileSize) {
            const parts = Math.ceil(file.size / chunkSize);
            selectedFileSize.textContent = `(${formatBytes(file.size)} • ${parts} parça)`;
        }
        if (fileInfo) fileInfo.classList.remove("d-none");
    }

    if (dropZone) {
        dropZone.addEventListener("click", () => fileInput.click());

        ["dragenter", "dragover"].forEach(evt => {
            dropZone.addEventListener(evt, (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropZone.classList.add("dragover");
            }, false);
        });

        ["dragleave", "drop"].forEach(evt => {
            dropZone.addEventListener(evt, (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropZone.classList.remove("dragover");
            }, false);
        });

        dropZone.addEventListener("drop", (e) => {
            const dt = e.dataTransfer;
            if (dt && dt.files && dt.files.length > 0) {
                setFile(dt.files[0]);
            }
        });
    }

    fileInput.addEventListener("change", (e) => {
        if (e.target.files && e.target.files.length > 0) {
            setFile(e.target.files[0]);
        }
    });

    if (btnChangeFile) {
        btnChangeFile.addEventListener("click", (e) => {
            e.stopPropagation();
            fileInput.click();
        });
    }

    button.addEventListener("click", async () => {
        const file = selectedFile || (fileInput.files && fileInput.files[0]);

        if (!file || !file.name.toLowerCase().endsWith(".zip") || file.size > maxFileSize) {
            status.className = "mt-3 fw-semibold small text-danger";
            status.textContent = "Lütfen 300 MB'den küçük geçerli bir ZIP dosyası seçin.";
            return;
        }

        const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : "";
        const uploadId = window.crypto.randomUUID();
        const totalChunks = Math.ceil(file.size / chunkSize);

        button.disabled = true;
        button.innerHTML = `<i class="fas fa-spinner fa-spin"></i> Yükleniyor...`;
        progress.classList.remove("d-none");
        progressBar.style.width = "0%";
        progressBar.textContent = "0%";
        progressBar.classList.remove("bg-success");
        status.className = "mt-3 fw-semibold small text-primary";

        try {
            for (let index = 0; index < totalChunks; index++) {
                status.textContent = `Yükleniyor: ${index + 1}/${totalChunks} parça (${formatBytes((index + 1) * chunkSize > file.size ? file.size : (index + 1) * chunkSize)} / ${formatBytes(file.size)})`;

                const formData = new FormData();
                if (token) formData.append("__RequestVerificationToken", token);
                formData.append("yuklemeId", uploadId);
                formData.append("parcaNo", index);
                formData.append("toplamParca", totalChunks);
                formData.append("parca", file.slice(index * chunkSize, Math.min(file.size, (index + 1) * chunkSize)), `parca-${index}`);

                const response = await fetch("/Admin/Urun/KanvasGorselParcaYukle", {
                    method: "POST",
                    body: formData,
                    credentials: "same-origin"
                });

                if (!response.ok) {
                    const result = await response.json().catch(() => ({}));
                    throw new Error(result.error || `${index + 1}. parça yüklenemedi.`);
                }

                const percent = Math.round(((index + 1) / totalChunks) * 100);
                progressBar.style.width = `${percent}%`;
                progressBar.textContent = `${percent}%`;
            }

            status.textContent = "ZIP açılıyor ve kalıcı depolamaya kaydediliyor...";
            const completeData = new FormData();
            if (token) completeData.append("__RequestVerificationToken", token);
            completeData.append("yuklemeId", uploadId);
            completeData.append("toplamParca", totalChunks);

            const completeResponse = await fetch("/Admin/Urun/KanvasGorselYuklemeyiTamamla", {
                method: "POST",
                body: completeData,
                credentials: "same-origin"
            });

            const completeResult = await completeResponse.json().catch(() => ({}));
            if (!completeResponse.ok) {
                throw new Error(completeResult.error || "Görseller işlenemedi.");
            }

            progressBar.style.width = "100%";
            progressBar.textContent = "100%";
            progressBar.classList.add("bg-success");
            status.className = "mt-3 fw-semibold small text-success";
            status.textContent = completeResult.message || "Yükleme başarıyla tamamlandı.";
            button.innerHTML = `<i class="fas fa-check"></i> Tamamlandı`;
        } catch (error) {
            status.className = "mt-3 fw-semibold small text-danger";
            status.textContent = error.message || "Yükleme başarısız oldu.";
            button.disabled = false;
            button.innerHTML = `<i class="fas fa-cloud-arrow-up"></i> Yüklemeyi Başlat`;
        }
    });
})();
