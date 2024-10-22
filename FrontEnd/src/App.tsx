import { Route, Routes } from 'react-router-dom'
import './App.css'
import Login from './components/Login/Login'
import Register from './components/Login/Register'
import LayoutAdmin from './components/AdminConsole/LayoutAdmin'

function App() {

  return (
    <>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/admin/*" element={<LayoutAdmin />} />
      </Routes>
    </>
  )
}

export default App
