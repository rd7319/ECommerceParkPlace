import React, { useState, useEffect, useCallback } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ProductDto } from '../../types';
import { productService } from '../../services/productService';
import { useApp } from '../../context/AppContext';
import './ProductDetail.css';

const ProductDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<ProductDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [quantity, setQuantity] = useState(1);
  
  const { addToCart, isLoading: cartLoading } = useApp();

  const loadProduct = useCallback(async () => {
    if (!id) return;
    
    try {
      setLoading(true);
      const response = await productService.getProduct(parseInt(id));
      
      if (response.isSuccess) {
        setProduct(response.data || null);
        setError(null);
      } else {
        setError(response.message || 'Failed to load product');
      }
    } catch (err) {
      setError('Failed to load product');
      console.error('Error loading product:', err);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    loadProduct();
  }, [loadProduct]);

  const handleAddToCart = async () => {
    if (product) {
      await addToCart(product.id, quantity);
    }
  };

  if (loading) {
    return <div className="loading">Loading product...</div>;
  }

  if (error || !product) {
    return (
      <div className="error">
        <p>Error: {error || 'Product not found'}</p>
        <Link to="/products">← Back to Products</Link>
      </div>
    );
  }

  return (
    <div className="product-detail">
      <div className="container">
        <Link to="/products" className="back-link">← Back to Products</Link>
        
        <div className="product-detail-content">
          <div className="product-image-section">
            {product.imageUrl ? (
              <img src={product.imageUrl} alt={product.name} className="product-image" />
            ) : (
              <div className="placeholder-image">No Image Available</div>
            )}
          </div>

          <div className="product-info-section">
            <div className="breadcrumb">
              <span>{product.franchise.name}</span> › <span>{product.typeName}</span>
            </div>
            
            <h1 className="product-title">{product.name}</h1>
            
            <div className="product-meta">
              <span className="franchise-name">{product.franchise.name}</span>
              <span className="product-type">{product.typeName}</span>
            </div>

            <p className="product-price">₹{product.price.toFixed(2)}</p>

            {product.size && (
              <p className="product-size">Size: {product.size}</p>
            )}

            {product.color && (
              <p className="product-color">Color: {product.color}</p>
            )}

            <div className="stock-info">
              {product.stockQuantity > 0 ? (
                <span className="in-stock">In Stock ({product.stockQuantity} available)</span>
              ) : (
                <span className="out-of-stock">Out of Stock</span>
              )}
            </div>

            <p className="product-description">{product.description}</p>

            {product.isAvailable && product.stockQuantity > 0 && (
              <div className="purchase-section">
                <div className="quantity-selector">
                  <label htmlFor="quantity">Quantity:</label>
                  <select
                    id="quantity"
                    value={quantity}
                    onChange={(e) => setQuantity(parseInt(e.target.value))}
                  >
                    {Array.from({ length: Math.min(product.stockQuantity, 10) }, (_, i) => (
                      <option key={i + 1} value={i + 1}>
                        {i + 1}
                      </option>
                    ))}
                  </select>
                </div>

                <button
                  onClick={handleAddToCart}
                  disabled={cartLoading}
                  className="add-to-cart-btn"
                >
                  {cartLoading ? 'Adding to Cart...' : `Add to Cart - ₹${(product.price * quantity).toFixed(2)}`}
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetail;