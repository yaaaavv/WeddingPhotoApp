document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("commentModal");
    const modalPhoto = document.getElementById("modalPhoto");
    const nameInput = document.getElementById("commentName");
    const textInput = document.getElementById("commentText");
    const submitBtn = document.getElementById("submitComment");
    const closeBtn = document.querySelector(".close-btn");
    let currentPhotoId = null;

    // 写真クリックでモーダル表示
    document.querySelectorAll(".thumbnail").forEach(img => {
        img.addEventListener("click", function () {
            currentPhotoId = this.dataset.photoId;
            modalPhoto.src = this.src;
            modal.style.display = "block";
        });
    });

    // モーダル閉じる
    closeBtn.addEventListener("click", () => {
        modal.style.display = "none";
    });

    // コメント送信
    submitBtn.addEventListener("click", () => {
        const name = nameInput.value.trim();
        const text = textInput.value.trim();

        if (!name || !text) {
            alert("名前とコメントを入力してください！");
            return;
        }

        fetch("/photo/comment/ajax", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                photoId: currentPhotoId,
                userName: name,
                text: text
            })
        })
            .then(res => {
                if (res.ok) {
                    alert("コメントを送信しました！");
                    nameInput.value = "";
                    textInput.value = "";
                    modal.style.display = "none";
                } else {
                    alert("送信に失敗しました");
                }
            });
    });
});
