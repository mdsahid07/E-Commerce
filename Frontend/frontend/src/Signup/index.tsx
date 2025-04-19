import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './styles.css';

interface SignupForm {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  address: string;
}

const apiUrl = import.meta.env.VITE_API_URL;

const Signup: React.FC = () => {
  const [formData, setFormData] = useState<SignupForm>({ username: '', email: '', password: '', confirmPassword: '', address: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords don't match!");
      setLoading(false);
      return;
    }

    try {
      const response = await fetch(apiUrl +'/dev/signup', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: formData.email,
          password: formData.password,
          username: formData.username
        }),
      });
      if (!response.ok) {
        throw new Error('Signup failed');
      }
      const data = await response.json();
      console.log('Signup successful:', data);
      navigate('/login');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred during signup');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="signup-page-container">
      <div className="signup-form-container">
        <h2 className="signup-form-heading">Sign Up</h2>
        <form onSubmit={handleSubmit}>
        <div className="signup-field-container">
            <label className="signup-form-label" htmlFor="username">
              Username
            </label>
            <input
              type="username"
              name="username"
              id="username"
              value={formData.username}
              onChange={handleChange}
              className="signup-form-input"
              required
            />
          </div>
          <div className="signup-field-container">
            <label className="signup-form-label" htmlFor="email">
              Email
            </label>
            <input
              type="email"
              name="email"
              id="email"
              value={formData.email}
              onChange={handleChange}
              className="signup-form-input"
              required
            />
          </div>
          <div className="signup-field-container">
            <label className="signup-form-label" htmlFor="password">
              Password
            </label>
            <input
              type="password"
              name="password"
              id="password"
              value={formData.password}
              onChange={handleChange}
              className="signup-form-input"
              required
            />
          </div>
          <div className="signup-field-container-last">
            <label className="signup-form-label" htmlFor="confirmPassword">
              Confirm Password
            </label>
            <input
              type="password"
              name="confirmPassword"
              id="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className="signup-form-input"
              required
            />
          </div>
          {error && <div style={{color: "red"}}>{error}</div>}
          <button type="submit" className="signup-form-button">
            Sign Up
          </button>
          {loading ? 'Signing up...' : 'Signup'}
          <p className="signup-link-paragraph">
            Already have an account?{' '}
            <Link to="/login" className="signup-form-link">
              Login
            </Link>
          </p>
        </form>
      </div>
    </div>
  );
};

export default Signup;