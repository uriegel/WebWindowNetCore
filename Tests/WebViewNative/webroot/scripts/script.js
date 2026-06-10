const params = new URLSearchParams(window.location.search);

const platform = params.get('platform')
if (platform != "windows") {
    const titlebar = document.getElementsByClassName("titlebar")[0]
    titlebar.classList.add("hidden")
}
const minimize = document.getElementById("minimize")
minimize.onclick = () => alert("minimize")
const maximize = document.getElementById("maximize")
maximize.onclick = () => alert("maximize")
const restore = document.getElementById("restore")
restore.onclick = () => alert("restore")
const close = document.getElementById("close")
close.onclick = () => window.close()