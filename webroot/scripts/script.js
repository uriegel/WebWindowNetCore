const source = new EventSource("http://localhost:20000/sse/test")
source.onmessage = (event) => console.log("SSE event", event.data)

const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btnDevTools = document.getElementById("buttonDevTools")

btnDevTools.onclick = webViewShowDevTools

// btn1.onclick = async () => {
//     var res = await request("cmd1", {
//         text: "Text",
//         id: 123
//     })
//     console.log("cmd1", res)
// }

// async function request(method, input) {

//     const msg = {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify(input)
//     }

//     const response = await fetch(`/json/${method}`, msg) 
//     return await response.json() 
// }


