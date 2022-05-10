////window.onload = function print() {
////    document.getElementById("test").innerHTML = "test";
////}

window.onload = function setSize() {
    document.getElementById("render").style.marginTop = document.getElementById("mainNavbar").style.height;
}


window.addEventListener("load", () => {
    const loader = document.querySelector(".loader");

    loader.classList.add("loader--hidden");

    loader.addEventListener("transitionend", () => {
        if (document.querySelector(".loader")) {
            document.body.removeChild(loader)
        }
    })
});
