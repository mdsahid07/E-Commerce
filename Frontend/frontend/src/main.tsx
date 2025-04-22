import { createRoot } from 'react-dom/client'
import './index.css'
import Home from './Home';
// import Login from './Login/index.tsx';
// import Signup from './Signup/index.tsx';
import { BrowserRouter, Routes, Route} from 'react-router-dom';
import { AuthProvider } from 'react-oidc-context';

const cognitoAuthConfig = {
  authority: import.meta.env.VITE_API_AUTHORITY,
  client_id: import.meta.env.VITE_API_CLIENT_ID,
  redirect_uri: import.meta.env.VITE_API_LOGOUT_URI,
  response_type: "code",
  scope: "email openid phone",
};

createRoot(document.getElementById('root')!).render(
  <AuthProvider {...cognitoAuthConfig}>  
      <BrowserRouter>
        <Routes>
          {/* <Route path="/login" element={<Login />} /> */}
          {/* <Route path="/signup" element={<Signup />} /> */}
          {/* <Route path="/home" element={<Home />} /> */}
          <Route
            path="/"
            element={
                <Home />
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
)
