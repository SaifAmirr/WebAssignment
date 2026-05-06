import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function ProtectedRoute({ children, roles }) {
  const { user, loading } = useAuth();

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );

  if (!user) return <Navigate to="/login" replace />;

  if (user.role === 'Pending') return <Navigate to="/" replace />;

  if (roles && !roles.includes(user.role)) {
    return (
      <div className="container mt-5 text-center">
        <h4 className="text-danger">Access Denied</h4>
        <p className="text-muted">You do not have permission to view this page.</p>
      </div>
    );
  }

  return children;
}
