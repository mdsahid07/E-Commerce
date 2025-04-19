import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './styles.css';

interface SignupForm {
  email: string;
  password: string;
  confirmPassword: string;
  address: string;
}

const Signup: React.FC = () => {
  const [formData, setFormData] = useState<SignupForm>({ email: '', password: '', confirmPassword: '', address: '' });
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleChangeTextArea = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.password !== formData.confirmPassword) {
      alert("Passwords don't match!");
      return;
    }
    console.log('Signup submitted:', formData);
    // Add your signup logic here
    navigate('/login');
  };

  return (
    <div className="signup-page-container">
      <div className="signup-form-container">
        <h2 className="signup-form-heading">Sign Up</h2>
        <form onSubmit={handleSubmit}>
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
          <div className="signup-field-container">
            <label className="signup-form-label" htmlFor="address">
            Address
            </label>
            <textarea
              name="address"
              id="address"
              value={formData.address}
              onChange={handleChangeTextArea}
              className="signup-form-input"
              required
            />
          </div>
          <button type="submit" className="signup-form-button">
            Sign Up
          </button>
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