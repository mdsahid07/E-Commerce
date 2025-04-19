import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './styles.css';

interface LoginForm {
  email: string;
  password: string;
}

const apiUrl = import.meta.env.VITE_API_URL;

const Login: React.FC = () => {
  const [formData, setFormData] = useState<LoginForm>({ email: '', password: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const cachedUser = localStorage.getItem('authToken');
    if (cachedUser) 
      navigate('/');
  }, [navigate]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(apiUrl + '/dev/signin', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData),
      });

      console.log('resp: ' + JSON.stringify(response));
      if (!response.ok) {
        throw new Error('Login failed');
      }

      const data = await response.json();
        localStorage.setItem('authToken', data.token);
        navigate('/');
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
  };

  return (
    <div className="login-page-container">
      <div className="login-form-container">
        <h2 className="login-form-heading">Login</h2>
        <form onSubmit={handleSubmit}>
          <div className="login-field-container">
            <label className="login-form-label" htmlFor="email">
              Email
            </label>
            <input
              type="email"
              name="email"
              id="email"
              value={formData.email}
              onChange={handleChange}
              className="login-form-input"
              required
            />
          </div>
          <div className="login-field-container">
            <label className="login-form-label" htmlFor="password">
              Password
            </label>
            <input
              type="password"
              name="password"
              id="password"
              value={formData.password}
              onChange={handleChange}
              className="login-form-input"
              required
            />
          </div>
          {error && <div style={{color: "red"}}>{error}</div>}
          <button type="submit" className="login-form-button">
            Login
          </button>
          {loading ? 'Logging in...' : 'Login'}
          <p className="login-link-paragraph">
            Don't have an account?{' '}
            <Link to="/signup" className="login-form-link">
              Sign Up
            </Link>
          </p>
        </form>
      </div>
    </div>
  );
};

export default Login;