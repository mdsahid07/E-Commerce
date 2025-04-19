import React, { useEffect, useState } from 'react';
import Card from '../Card';
import { Item } from '../models/Item';
import { useNavigate } from 'react-router-dom';
import './styles.css';

interface CartItem extends Item {
  quantity: number;
}

const Home: React.FC = () => {
  const [items, setItems] = useState<Item[]>([]);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [orderSuccess, setOrderSuccess] = useState<string | null>(null);
  const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false);
  const [token, setToken] = useState<string|null>();
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    setToken(token);
    setIsLoggedIn(!!token);
    const fetchItems = async () => {
      try {
        const response = await fetch('https://pvfz8ptao9.execute-api.us-east-1.amazonaws.com/dev/getproduct');
        if (!response.ok) 
          throw new Error('Failed to fetch items');
        const data: Item[] = await response.json();
        setItems(data);
        setLoading(false);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
        setLoading(false);
      }
    };
    fetchItems();
  }, []);

  const addToCart = (item: Item) => {
    setCart(prevCart => {
      const existingItem = prevCart.find(cartItem => cartItem.ProductId === item.ProductId);
      if (existingItem) {
        return prevCart.map(cartItem =>
          cartItem.ProductId === item.ProductId
            ? { ...cartItem, quantity: cartItem.quantity + 1 }
            : cartItem
        );
      }
      return [...prevCart, { ...item, quantity: 1 }];
    });
  };

  const removeFromCart = (itemId: string) => {
    setCart(prevCart => prevCart.filter(cartItem => cartItem.ProductId !== itemId));
  };

  const placeOrder = async() => {
    if (!isLoggedIn) {
      navigate('/login');
      return;
    }
    const total = cart
      .reduce((total, item) => total + item.Price * item.quantity, 0)
      .toFixed(2);
      try {
        const response = await fetch('https://pvfz8ptao9.execute-api.us-east-1.amazonaws.com/dev/placeorder', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer' + ' ' + token
          },
          body: JSON.stringify({ 
            "items": cart,
            "shippingAddress": ""
          }),
        });
        if (!response.ok)
          throw new Error('Order placement failed');
        setOrderSuccess(`Order placed successfully! Total amount: $${total}`);
        setCart([]);
        setTimeout(() => setOrderSuccess(null), 5000);
      } catch (error) {
        setOrderSuccess('Failed to place order. Please try again.');
        setTimeout(() => setOrderSuccess(null), 5000);
      }
  };

  const filteredItems = items.filter(item =>
    item.Name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    item.Description.toLowerCase().includes(searchQuery.toLowerCase()) 
  );

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    navigate('/login');
  };

  return (
    <div className="app">
      <div className="main-content">
      <button onClick={handleLogout}>Logout</button>
        <h2>Product List</h2>
        <div style={{ textAlign: "end", marginBottom: "20px" }}>
          <input
            id="search-input"
            type="text"
            aria-label="Search products"
            placeholder="Search products..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="button">
            Search
          </button>
        </div>
        {loading && <div>Loading...</div>}
        {error && <div>Error: {error}</div>}
        {!loading && !error && (
          <div className="card-grid">
            {filteredItems.map((item, index) => (
              <Card
                key={item.Name + index}
                item={item}
                className="card-item"
                onAddToCart={() => addToCart(item)}
              />
            ))}
          </div>
        )}
      </div>

      <div className="cart-sidebar">
        <h3>Shopping Cart</h3>
        {orderSuccess && <div className="success-message">{orderSuccess}</div>}
        {cart.length === 0 ? (
          <p>Your cart is empty</p>
        ) : (
          <div className="cart-items">
            {cart.map((cartItem) => (
              <div key={cartItem.ProductId} className="cart-item">
                <div>
                  <h4>{cartItem.Name}</h4>
                  <p>Quantity: {cartItem.quantity}</p>
                  <p>Price: ${(cartItem.Price * cartItem.quantity).toFixed(2)}</p>
                </div>
                <button
                  onClick={() => removeFromCart(cartItem.ProductId)}
                  className="remove-button"
                >
                  Remove
                </button>
              </div>
            ))}
            <div className="cart-total">
              <h4>
                Total: $
                {cart
                  .reduce((total, item) => total + item.Price * item.quantity, 0)
                  .toFixed(2)}
              </h4>
            </div>
            <button
              onClick={placeOrder}
              className="order-button"
              disabled={cart.length === 0}
            >
              Place Order
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;