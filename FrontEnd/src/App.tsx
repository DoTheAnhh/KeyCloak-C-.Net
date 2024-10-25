import { Route, Routes } from 'react-router-dom'
import './App.css'
import Login from './components/Login/Login'
import Register from './components/Register/Register'
import ListUser from './components/User/ListUser'
import Callback from './components/Login/Callback/Callback'

function App() {

  return (
    <>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/user" element={<ListUser />} />
        <Route path="/callback" element={<Callback />} />
      </Routes>
    </>
  )
}

export default App
