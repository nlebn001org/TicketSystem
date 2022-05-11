
window.addEventListener("load", () => {
    const loader = document.querySelector(".loader");

    loader.classList.add("loader--hidden");

    loader.addEventListener("transitionend", () => {
        if (document.querySelector(".loader")) {
            document.body.removeChild(loader)
        }
    })
});
