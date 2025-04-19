import { createRoot } from 'react-dom/client'
import './index.css'
import Home from './Home';
import Login from './Login/index.tsx';
import Signup from './Signup/index.tsx';
import { BrowserRouter, Routes, Route, Navigate} from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext.tsx';

createRoot(document.getElementById('root')!).render(
  <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/home" element={<Home />} />
          {/* <Route
            path="/main"
            element={
              <ProtectedRoute>
                <Home />
              </ProtectedRoute>
            }
          /> */}
          <Route path="/" element={<Navigate to="/home" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
)
