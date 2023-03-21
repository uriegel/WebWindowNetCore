import logo from './logo.svg';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <video controls autoPlay
            src="http://localhost:20000/video" />        
      </header>
    </div>
  );
}

export default App;
