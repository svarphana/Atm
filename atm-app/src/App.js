import logo from './logo.svg';
import './App.css';
import { Container, Typography } from '@material-ui/core';
import ATMachine from './components/Atm';

function App() {
  return (
    <Container maxWidth="md">
      <Typography
      gutterBottom
      variant="h2"
      align="center">
        ATM App
      </Typography>
      <ATMachine>
        
      </ATMachine>
    </Container>
    /*<div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>*/
  );
}

export default App;
