import React, { useState, useEffect } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { ProductDto, ProductType } from '../../types';
import { productService } from '../../services/productService';
import { useApp } from '../../context/AppContext';
import './Search.css';

const Search: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState(searchParams.get('q') || '');
  const [selectedType, setSelectedType] = useState<ProductType | undefined>();
  const [selectedFranchise, setSelectedFranchise] = useState<number | undefined>();
  
  const { franchises, addToCart, isLoading: cartLoading } = useApp();

  useEffect(() => {
    if (searchTerm || selectedType !== undefined || selectedFranchise !== undefined) {
      performSearch();
    }
  }, [searchTerm, selectedType, selectedFranchise]);

  const performSearch = async () => {
    try {
      setLoading(true);
      const response = await productService.searchProducts(
        searchTerm || undefined,
        selectedType,
        selectedFranchise
      );
      
      if (response.isSuccess) {
        setProducts(response.data || []);
        setError(null);
      } else {
        setError(response.message || 'Failed to search products');
      }
    } catch (err) {
      setError('Failed to search products');
      console.error('Error searching products:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const newSearchParams = new URLSearchParams();
    if (searchTerm) {
      newSearchParams.set('q', searchTerm);
    }
    setSearchParams(newSearchParams);
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

  return (
    <div className="search">
      <div className="container">
        <div className="search-header">
          <h1>Search Products</h1>
          
          <form onSubmit={handleSearchSubmit} className="search-form">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Search by product name, description, or franchise..."
              className="search-input"
            />
            <button type="submit" className="search-button">
              Search
            </button>
          </form>
        </div>

        <div className="search-filters">
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

          <button
            onClick={() => {
              setSelectedType(undefined);
              setSelectedFranchise(undefined);
              setSearchTerm('');
              setSearchParams({});
            }}
            className="clear-filters-btn"
          >
            Clear All
          </button>
        </div>

        {loading && <div className="loading">Searching...</div>}

        {error && <div className="error">Error: {error}</div>}

        {!loading && !error && (
          <>
            <div className="search-results-header">
              <h2>
                {products.length === 0
                  ? 'No products found'
                  : `${products.length} product${products.length === 1 ? '' : 's'} found`}
              </h2>
            </div>

            {products.length > 0 && (
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
          </>
        )}
      </div>
    </div>
  );
};

export default Search;