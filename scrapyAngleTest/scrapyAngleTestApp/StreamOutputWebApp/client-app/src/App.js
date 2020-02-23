import React from 'react';
import  Layout  from "./components/Layout/Layout";

import { GlobalProvider } from "./Context/GlobalState";
// import './App.css';

function App() {
  return (
    <GlobalProvider>
      <Layout/>
    </GlobalProvider>
  );
}

export default App;
