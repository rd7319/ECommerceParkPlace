import React from 'react';
import { Link } from 'react-router-dom';
import { useApp } from '../../context/AppContext';
import './Cart.css';

const Cart: React.FC = () => {
  const { cart, updateCartItem, removeFromCart, clearCart, isLoading, createOrderAsync } = useApp();

  const handleQuantityChange = async (productId: number, newQuantity: number) => {
    if (newQuantity > 0) {
      await updateCartItem(productId, newQuantity);
    }
  };

  const handleRemoveItem = async (productId: number) => {
    await removeFromCart(productId);
  };

  const handleClearCart = async () => {
    if (window.confirm('Are you sure you want to clear your cart?')) {
      await clearCart();
    }
  };
  const handleCreateOrder = async () => {
    await createOrderAsync();
  }

  if (!cart || cart.cartItems.length === 0) {
    return (
      <div className="cart">
        <div className="container">
          <h1>Shopping Cart</h1>
          <div className="empty-cart">
            <p>Your cart is empty</p>
            <Link to="/products" className="continue-shopping">
              Continue Shopping
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="cart">
      <div className="container">
        <div className="cart-header">
          <h1>Shopping Cart ({cart.cartItems.length} items)</h1>
          <button onClick={handleClearCart} className="clear-cart-btn">
            Clear Cart
          </button>
        </div>

        <div className="cart-content">
          <div className="cart-items">
            {cart.cartItems.map((item) => (
              <div key={item.id} className="cart-item">
                <div className="item-image">
                  {item.product.imageUrl ? (
                    <img src={item.product.imageUrl} alt={item.product.name} />
                  ) : (
                    <div className="placeholder-image">No Image</div>
                  )}
                </div>

                <div className="item-details">
                  <Link to={`/products/${item.product.id}`} className="item-name">
                    {item.product.name}
                  </Link>
                  <p className="item-franchise">{item.product.franchise.name}</p>
                  <p className="item-type">{item.product.typeName}</p>
                  {item.product.size && <p className="item-size">Size: {item.product.size}</p>}
                  {item.product.color && <p className="item-color">Color: {item.product.color}</p>}
                </div>

                <div className="item-price">
                  <span className="unit-price">₹{item.product.price.toFixed(2)}</span>
                </div>

                <div className="item-quantity">
                  <button
                    onClick={() => handleQuantityChange(item.productId, item.quantity - 1)}
                    disabled={isLoading || item.quantity <= 1}
                    className="quantity-btn"
                  >
                    -
                  </button>
                  <span className="quantity">{item.quantity}</span>
                  <button
                    onClick={() => handleQuantityChange(item.productId, item.quantity + 1)}
                    disabled={isLoading || item.quantity >= item.product.stockQuantity}
                    className="quantity-btn"
                  >
                    +
                  </button>
                </div>

                <div className="item-total">
                  <span className="total-price">₹{item.itemTotal.toFixed(2)}</span>
                </div>

                <div className="item-actions">
                  <button
                    onClick={() => handleRemoveItem(item.productId)}
                    disabled={isLoading}
                    className="remove-btn"
                  >
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>

          <div className="cart-summary">
            <div className="summary-card">
              <h3>Order Summary</h3>

              <div className="summary-line">
                <span>Subtotal ({cart.cartItems.length} items):</span>
                <span>₹{cart.totalAmount.toFixed(2)}</span>
              </div>

              <div className="summary-line">
                <span>Shipping:</span>
                <span>FREE</span>
              </div>

              <div className="summary-total">
                <span>Total:</span>
                <span>₹{cart.totalAmount.toFixed(2)}</span>
              </div>

              {/* <Link to="/checkout" className="checkout-btn">
                Proceed to Checkout
              </Link> */}
              <button
                onClick={() => handleCreateOrder()}
                disabled={isLoading}
                className="checkout-btn"
              >
                Proceed To Checkout
              </button>
              <Link to="/products" className="continue-shopping">
                Continue Shopping
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Cart;