import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useApp } from '../../context/AppContext';
import './Header.css';

const Header: React.FC = () => {
  const { cartItemCount , userId, setUserId} = useApp();
  const navigate = useNavigate();

  const handleSearchSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const searchTerm = formData.get('search') as string;
    if (searchTerm.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchTerm.trim())}`);
    }
  };
  const handleUserChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const value = Number(e.target.value);
    if (!isNaN(value)) setUserId(value);
  };
  return (
    <header className="header">
      <div className="container">
        <div className="header-content">
          <Link to="/" className="logo">
            <h1>IPL Store</h1>
          </Link>

          <nav className="nav">
            <Link to="/products" className="nav-link">
              Products
            </Link>
          </nav>

          <form className="search-form" onSubmit={handleSearchSubmit}>
            <input
              type="text"
              name="search"
              placeholder="Search products..."
              className="search-input"
            />
            <button type="submit" className="search-button">
              Search
            </button>
          </form>

          <div className="header-actions">
            <Link to="/cart" className="cart-link">
              Cart ({cartItemCount})
            </Link>
            <Link to="/orders" className="nav-link">
              Orders
            </Link>
            <div className="user-selector">
              <label htmlFor="userId" className="user-label"></label>
              <select
                id="userId"
                className="user-select"
                value={userId}
                onChange={(e) => handleUserChange(e)}
              >
                <option value="1">User 1</option>
                <option value="2">User 2</option>
                <option value="3">User 3</option>
              </select>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;