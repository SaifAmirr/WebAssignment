import React, { createContext, useContext, useState, useEffect } from 'react';
import { login as loginApi, register as registerApi } from '../services/authService';
import { decodeToken, getRole, getStudentId, getInstructorId } from '../utils/jwt';

const AuthContext = createContext(null);

function buildUserFromToken(token, username) {
  const payload = decodeToken(token);
  return {
    username,
    role: getRole(payload),
    studentId: getStudentId(payload),
    instructorId: getInstructorId(payload),
  };
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(() => localStorage.getItem('token'));

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedUser  = localStorage.getItem('user');
    if (storedToken && storedUser) {
      const base = JSON.parse(storedUser);
      const u = buildUserFromToken(storedToken, base.username);
      // Restore manually linked student record if the JWT has no StudentId claim
      if (!u.studentId && base.linkedStudentId) u.studentId = base.linkedStudentId;
      setUser(u);
    }
  }, []);

  const _persist = (token, username, extra = {}) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify({ username, ...extra }));
  };

  const login = async (credentials) => {
    const res = await loginApi(credentials);
    const { token, username } = res.data;
    _persist(token, username);
    setToken(token);
    setUser(buildUserFromToken(token, username));
    return res.data;
  };

  const register = async (data) => {
    const res = await registerApi(data);
    const { token, username } = res.data;
    _persist(token, username);
    setToken(token);
    setUser(buildUserFromToken(token, username));
    return res.data;
  };

  // Called when a student manually picks their student record from the list
  const linkStudentId = (studentId) => {
    const stored = JSON.parse(localStorage.getItem('user') || '{}');
    localStorage.setItem('user', JSON.stringify({ ...stored, linkedStudentId: studentId }));
    setUser((prev) => ({ ...prev, studentId }));
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setToken(null);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, token, login, register, logout, linkStudentId }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
