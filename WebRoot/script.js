const button = document.getElementById("button")
button.onclick = () => webWindowNetCore.postMessage("Guten Abend!👌👌👌😜") 


webWindowNetCore.setCallback(text => alert(text))

