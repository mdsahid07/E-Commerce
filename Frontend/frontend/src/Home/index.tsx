import React, { useEffect, useState } from 'react';
import Card from '../Card';
import { Item } from '../models/Item';
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

  const dummyDatas: Item[] = [
    {
      "ProductId": "abc123",
      "Name": "Wireless Mouse",
      "Description": "Ergonomic wireless mouse with USB receiver",
      "Price": 25.99,
      "ImageUrl": "https://your-s3-bucket-url/mouse.jpg",
      "CreatedAt": "2025-04-12T22:35:10.123Z"
    },
    {
      "ProductId": "xyz456",
      "Name": "Bluetooth Headphones",
      "Description": "Noise cancelling over-ear headphones",
      "Price": 89.99,
      "ImageUrl": "https://your-s3-bucket-url/headphones.jpg",
      "CreatedAt": "2025-04-11T17:12:55.789Z"
    }
  ];

  useEffect(() => {
    const fetchItems = async () => {
      try {
        const response = await fetch('https://dummyjson.com/products');
        if (!response.ok) 
          throw new Error('Failed to fetch items');
        // const data: Item[] = await response.json().then(body => body.products);
        const data = dummyDatas;
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

  const placeOrder = () => {
    const total = cart
      .reduce((total, item) => total + item.Price * item.quantity, 0)
      .toFixed(2);
    setOrderSuccess(`Order placed successfully! Total amount: $${total}`);
    setCart([]);
    setTimeout(() => setOrderSuccess(null), 5000);
  };

  const filteredItems = items.filter(item =>
    item.Name.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="app">
      <div className="main-content">
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