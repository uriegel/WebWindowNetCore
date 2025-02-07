import { useState } from 'react'
import reactLogo from '/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import './webview.ts'
import { WebViewType } from './webview.ts'

declare var WebView: WebViewType

type Input = {
    text: string,
    id: number
}

type Contact = {
    name: string,
    id: number
}

function App() {
  const [count, setCount] = useState(0)

  const onRequest = async () => {
    var res = await WebView.request<Input, Contact>("cmd1", { id: 98, text: "Uwe" })
    console.log(res, res.name)
  }

  const onDevtools = () => WebView.showDevTools() 

  return (
    <>
      <div>
        <img src="webroot/images/image.jpg"/>
        <a href="https://vitejs.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>count is {count}</button>
        <button onClick={onRequest}>Request</button>
        <button onClick={onDevtools}>Dev tool</button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
