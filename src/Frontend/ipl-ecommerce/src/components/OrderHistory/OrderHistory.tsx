import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { OrderDto } from '../../types';
import { orderService } from '../../services/orderService';
import { useApp } from '../../context/AppContext';
import './OrderHistory.css';

const OrderHistory: React.FC = () => {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  const { userId } = useApp();

  useEffect(() => {
    loadOrders();
  }, [userId]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const response = await orderService.getUserOrders(userId);
      
      if (response.isSuccess) {
        setOrders(response.data || []);
        setError(null);
      } else {
        setError(response.message || 'Failed to load orders');
      }
    } catch (err) {
      setError('Failed to load orders');
      console.error('Error loading orders:', err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-IN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'processing':
        return 'status-processing';
      case 'shipped':
        return 'status-shipped';
      case 'delivered':
        return 'status-delivered';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return 'status-default';
    }
  };

  if (loading) {
    return <div className="loading">Loading orders...</div>;
  }

  if (error) {
    return <div className="error">Error: {error}</div>;
  }

  return (
    <div className="order-history">
      <div className="container">
        <div className="page-header">
          <h1>Order History</h1>
          <p>View and track your previous orders</p>
        </div>

        {orders.length === 0 ? (
          <div className="no-orders">
            <p>You haven't placed any orders yet.</p>
            <Link to="/products" className="start-shopping">
              Start Shopping
            </Link>
          </div>
        ) : (
          <div className="orders-list">
            {orders.map((order) => (
              <div key={order.id} className="order-card">
                <div className="order-header">
                  <div className="order-info">
                    <h3 className="order-number">Order #{order.orderNumber}</h3>
                    <p className="order-date">Placed on {formatDate(order.orderDate)}</p>
                  </div>
                  <div className="order-status">
                    <span className={`status-badge ${getStatusColor(order.statusName)}`}>
                      {order.statusName}
                    </span>
                  </div>
                </div>

                <div className="order-details">
                  <div className="order-items">
                    <h4>Items ({order.orderItems.length})</h4>
                    <div className="items-list">
                      {order.orderItems.map((item) => (
                        <div key={item.id} className="order-item">
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
                            <p className="item-specs">
                              Quantity: {item.quantity} × ₹{item.unitPrice.toFixed(2)} = ₹{item.totalPrice.toFixed(2)}
                            </p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="order-summary">
                    <div className="summary-row">
                      <span>Total Amount:</span>
                      <span className="total-amount">₹{order.totalAmount.toFixed(2)}</span>
                    </div>
                    {order.trackingNumber && (
                      <div className="summary-row">
                        <span>Tracking Number:</span>
                        <span className="tracking-number">{order.trackingNumber}</span>
                      </div>
                    )}
                    <div className="summary-row">
                      <span>Shipping Address:</span>
                      <span className="shipping-address">{order.shippingAddress}</span>
                    </div>
                  </div>
                </div>

                <div className="order-actions">
                  <Link to={`/orders/${order.id}`} className="view-details-btn">
                    View Details
                  </Link>
                  {order.statusName.toLowerCase() === 'delivered' && (
                    <button className="reorder-btn">
                      Order Again
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderHistory;