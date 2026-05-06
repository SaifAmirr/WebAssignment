import React, { createContext, useContext, useState, useEffect } from 'react';
import { login as loginApi, register as registerApi, getMe, logoutApi } from '../services/authService';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser]       = useState(null);
  const [loading, setLoading] = useState(true); // true while checking session on mount

  const studentKey    = (username) => `linkedStudentId_${username}`;
  const instructorKey = (username) => `linkedInstructorId_${username}`;

  // Apply any locally-stored linked IDs that the backend doesn't know about yet
  const applyLinkedIds = (u) => {
    if (!u.studentId) {
      const saved = localStorage.getItem(studentKey(u.username));
      if (saved) u.studentId = Number(saved);
    }
    if (!u.instructorId) {
      const saved = localStorage.getItem(instructorKey(u.username));
      if (saved) u.instructorId = Number(saved);
    }
    return u;
  };

  // On mount: ask the backend if the cookie is still valid
  useEffect(() => {
    getMe()
      .then((res) => setUser(applyLinkedIds({ ...res.data })))
      .catch(() => setUser(null))
      .finally(() => setLoading(false));
  }, []);

  const login = async (credentials) => {
    const res = await loginApi(credentials);
    setUser(applyLinkedIds({ ...res.data }));
    return res.data;
  };

  const register = async (data) => {
    const res = await registerApi(data);
    return res.data; // no auto-login; caller handles redirect
  };

  const linkStudentId = (studentId) => {
    localStorage.setItem(studentKey(user.username), studentId);
    setUser((prev) => ({ ...prev, studentId }));
  };

  const linkInstructorId = (instructorId) => {
    localStorage.setItem(instructorKey(user.username), instructorId);
    setUser((prev) => ({ ...prev, instructorId }));
  };

  const logout = async () => {
    await logoutApi().catch(() => {});
    setUser(null);
    // linkedStudentId_<username> intentionally kept so the link is restored on next login
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout, linkStudentId, linkInstructorId }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
