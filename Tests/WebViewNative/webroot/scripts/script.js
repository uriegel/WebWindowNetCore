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
const devtools = document.getElementById("devtools")
devtools.onclick = () => alert("devtools")

function onMaximized(value) 
{
    console.log("Maximized", value)
    if (isMaximized != value) {
        isMaximized = value
        if (isMaximized) {
            maximize.classList.add("hidden")
            restore.classList.remove("hidden")
        } else {
            maximize.classList.remove("hidden")
            restore.classList.add("hidden")
        }

    }
}

alert("ready")

var isMaximized