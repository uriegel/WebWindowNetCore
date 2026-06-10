const params = new URLSearchParams(window.location.search);

const platform = params.get('platform')
if (platform != "windows") {
    const titlebar = document.getElementsByClassName("titlebar")[0]
    titlebar.classList.add("hidden")
}
const close = document.getElementById("close")
close.onclick = () => window.close()