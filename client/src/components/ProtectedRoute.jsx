import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function ProtectedRoute({ children, roles }) {
  const { token, user } = useAuth();

  if (!token) return <Navigate to="/login" replace />;

  if (roles && user && !roles.includes(user.role)) {
    return (
      <div className="container mt-5 text-center">
        <h4 className="text-danger">Access Denied</h4>
        <p className="text-muted">You do not have permission to view this page.</p>
      </div>
    );
  }

  return children;
}
