import { Route, Routes } from 'react-router-dom'
import './App.css'
import Login from './components/Login'
import Success from './components/Success/Success'
import Register from './components/Register'

function App() {

  return (
    <>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/success" element={<Success />} />
      </Routes>
    </>
  )
}

export default App
