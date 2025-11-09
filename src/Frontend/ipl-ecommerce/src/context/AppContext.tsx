import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { ProductDto, CartDto, FranchiseDto, CreateOrderRequest } from '../types';
import { cartService } from '../services/cartService';
import { franchiseService } from '../services/franchiseService';
import { orderService } from '../services/orderService';
import { useNavigate } from 'react-router-dom';

interface AppContextType {
  userId: number;
  cart: CartDto | null;
  cartItemCount: number;
  franchises: FranchiseDto[];
  isLoading: boolean;
  error: string | null;
  refreshCart: () => Promise<void>;
  addToCart: (productId: number, quantity: number) => Promise<void>;
  updateCartItem: (productId: number, quantity: number) => Promise<void>;
  removeFromCart: (productId: number) => Promise<void>;
  clearCart: () => Promise<void>;
  createOrderAsync: () => Promise<void>;
  setUserId: React.Dispatch<React.SetStateAction<number>>;
  setError: (error: string | null) => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

interface AppProviderProps {
  children: ReactNode;
}

export const AppProvider: React.FC<AppProviderProps> = ({ children }) => {

  const [userId, setUserId] = useState(1);
  const [cart, setCart] = useState<CartDto | null>(null);
  const [franchises, setFranchises] = useState<FranchiseDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const cartItemCount = cart?.cartItems.reduce((total, item) => total + item.quantity, 0) || 0;

  const refreshCart = async () => {
    try {
      setIsLoading(true);
      const response = await cartService.getUserCart(userId);
      if (response.isSuccess) {
        setCart(response.data || null);
      } else {
        setError(response.message || 'Failed to load cart');
      }
    } catch (err) {
      setError('Failed to load cart');
      console.error('Error loading cart:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const loadFranchises = async () => {
    try {
      const response = await franchiseService.getAllFranchises();
      if (response.isSuccess) {
        setFranchises(response.data || []);
      } else {
        setError(response.message || 'Fa</svg>iled to load franchises');
      }
    } catch (err) {
      console.error('Error loading franchises:', err);
    }
  };

  const addToCart = async (productId: number, quantity: number) => {
    try {
      setIsLoading(true);
      const response = await cartService.addToCart({
        userId,
        productId,
        quantity,
      });

      if (response.isSuccess) {
        setCart(response.data || null);
        setError(null);
      } else {
        setError(response.message || 'Failed to add item to cart');
      }
    } catch (err) {
      setError('Failed to add item to cart');
      console.error('Error adding to cart:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const updateCartItem = async (productId: number, quantity: number) => {
    try {
      setIsLoading(true);
      const response = await cartService.updateCartItem({
        userId,
        productId,
        quantity,
      });

      if (response.isSuccess) {
        setCart(response.data || null);
        setError(null);
      } else {
        setError(response.message || 'Failed to update cart item');
      }
    } catch (err) {
      setError('Failed to update cart item');
      console.error('Error updating cart item:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const removeFromCart = async (productId: number) => {
    try {
      setIsLoading(true);
      const response = await cartService.removeFromCart(userId, productId);

      if (response.isSuccess) {
        setCart(response.data || null);
        setError(null);
      } else {
        setError(response.message || 'Failed to remove item from cart');
      }
    } catch (err) {
      setError('Failed to remove item from cart');
      console.error('Error removing from cart:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const clearCart = async () => {
    try {
      setIsLoading(true);
      const response = await cartService.clearCart(userId);

      if (response.isSuccess) {
        setCart(response.data || null);
        setError(null);
      } else {
        setError(response.message || 'Failed to clear cart');
      }
    } catch (err) {
      setError('Failed to clear cart');
      console.error('Error clearing cart:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const createOrderAsync = async () => {
    try {
      var order: CreateOrderRequest = { userId: userId, shippingAddress: "Hyderabad" };
      setIsLoading(true);
      const response = await orderService.createOrder(order);

      if (response.isSuccess) {
        await refreshCart();
        navigate('/orders')
        setError(null);
      } else {
        setError(response.message || 'Failed to place order');
      }
    } catch (err) {
      setError('Failed to place order');
      console.error('Error placing order', err);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    refreshCart();
    loadFranchises();
  }, []);
  useEffect(() => {
    refreshCart();
    loadFranchises();
  }, [userId]);

  const value: AppContextType = {
    userId,
    cart,
    cartItemCount,
    franchises,
    isLoading,
    error,
    refreshCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart,
    setError,
    setUserId,
    createOrderAsync,
  };

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
};

export const useApp = (): AppContextType => {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error('useApp must be used within an AppProvider');
  }
  return context;
};