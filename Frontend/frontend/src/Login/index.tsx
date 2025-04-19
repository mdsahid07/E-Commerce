import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './styles.css';

interface LoginForm {
  email: string;
  password: string;
}

const Login: React.FC = () => {
  const [formData, setFormData] = useState<LoginForm>({ email: '', password: '' });
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log('Login submitted:', formData);
    // Add your login logic here
    navigate('/');
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
          <button type="submit" className="login-form-button">
            Login
          </button>
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