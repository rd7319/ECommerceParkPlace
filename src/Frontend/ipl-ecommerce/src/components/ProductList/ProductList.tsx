import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ProductDto, ProductType } from '../../types';
import { productService } from '../../services/productService';
import { useApp } from '../../context/AppContext';
import './ProductList.css';

const ProductList: React.FC = () => {
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedType, setSelectedType] = useState<ProductType | undefined>();
  const [selectedFranchise, setSelectedFranchise] = useState<number | undefined>();
  
  const { franchises, addToCart, isLoading: cartLoading } = useApp();

  useEffect(() => {
    loadProducts();
  }, [selectedType, selectedFranchise]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const response = await productService.searchProducts(
        undefined,
        selectedType,
        selectedFranchise
      );
      
      if (response.isSuccess) {
        setProducts(response.data || []);
        setError(null);
      } else {
        setError(response.message || 'Failed to load products');
      }
    } catch (err) {
      setError('Failed to load products');
      console.error('Error loading products:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCart = async (productId: number) => {
    await addToCart(productId, 1);
  };

  const getProductTypeOptions = () => {
    return Object.entries(ProductType)
      .filter(([key]) => !isNaN(Number(key)))
      .map(([key, value]) => ({
        value: Number(key),
        label: value as string,
      }));
  };

  if (loading) {
    return <div className="loading">Loading products...</div>;
  }

  if (error) {
    return <div className="error">Error: {error}</div>;
  }

  return (
    <div className="product-list">
      <div className="container">
        <div className="page-header">
          <h1>IPL Products</h1>
          <p>Shop authentic IPL franchise merchandise</p>
        </div>

        <div className="filters">
          <div className="filter-group">
            <label htmlFor="type-filter">Product Type:</label>
            <select
              id="type-filter"
              value={selectedType || ''}
              onChange={(e) => setSelectedType(e.target.value ? Number(e.target.value) : undefined)}
            >
              <option value="">All Types</option>
              {getProductTypeOptions().map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          <div className="filter-group">
            <label htmlFor="franchise-filter">Franchise:</label>
            <select
              id="franchise-filter"
              value={selectedFranchise || ''}
              onChange={(e) => setSelectedFranchise(e.target.value ? Number(e.target.value) : undefined)}
            >
              <option value="">All Franchises</option>
              {franchises.map((franchise) => (
                <option key={franchise.id} value={franchise.id}>
                  {franchise.name}
                </option>
              ))}
            </select>
          </div>

          <button onClick={() => { setSelectedType(undefined); setSelectedFranchise(undefined); }}>
            Clear Filters
          </button>
        </div>

        {products.length === 0 ? (
          <div className="no-products">
            <p>No products found matching your criteria.</p>
          </div>
        ) : (
          <div className="products-grid">
            {products.map((product) => (
              <div key={product.id} className="product-card">
                <Link to={`/products/${product.id}`} className="product-link">
                  <div className="product-image">
                    {product.imageUrl ? (
                      <img src={product.imageUrl} alt={product.name} />
                    ) : (
                      <div className="placeholder-image">No Image</div>
                    )}
                  </div>
                  <div className="product-info">
                    <h3 className="product-name">{product.name}</h3>
                    <p className="product-franchise">{product.franchise.name}</p>
                    <p className="product-type">{product.typeName}</p>
                    <p className="product-price">₹{product.price.toFixed(2)}</p>
                    {product.stockQuantity <= 5 && (
                      <p className="stock-warning">Only {product.stockQuantity} left!</p>
                    )}
                  </div>
                </Link>
                <div className="product-actions">
                  <button
                    onClick={() => handleAddToCart(product.id)}
                    disabled={!product.isAvailable || product.stockQuantity === 0 || cartLoading}
                    className="add-to-cart-btn"
                  >
                    {cartLoading ? 'Adding...' : 'Add to Cart'}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProductList;