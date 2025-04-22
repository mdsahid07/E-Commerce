import { createRoot } from 'react-dom/client'
import './index.css'
import Home from './Home';
import Login from './Login/index.tsx';
import Signup from './Signup/index.tsx';
import { BrowserRouter, Routes, Route, Navigate} from 'react-router-dom';
import { AuthProvider } from 'react-oidc-context';

const cognitoAuthConfig = {
  // authority: "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_nF0gmhHCd",
  // client_id: "2te7j1ap9idg4fbtjotildkqfo",
  // redirect_uri: "http://localhost:5173/home",
  // response_type: "code",
  // scope: "email openid phone",
};

createRoot(document.getElementById('root')!).render(
  <AuthProvider {...cognitoAuthConfig}>  
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/home" element={<Home />} />
          <Route
            path="/main"
            element={
                <Home />
            }
          />
          <Route path="/" element={<Navigate to="/home" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
)
