import React, { useState, useEffect } from 'react';
import { getAllUsers, updateUserRole } from '../services/authService';
import { useAuth } from '../context/AuthContext';

const ASSIGNABLE_ROLES = ['Student', 'Instructor', 'Admin'];

const ROLE_BADGE = {
  Admin:      'bg-danger',
  Instructor: 'bg-warning text-dark',
  Student:    'bg-success',
  Pending:    'bg-secondary',
};

export default function UsersPage() {
  const { user: currentUser } = useAuth();
  const [users, setUsers]     = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');
  const [msg, setMsg]         = useState('');
  const [saving, setSaving]   = useState(null);

  const load = () => {
    setLoading(true);
    getAllUsers()
      .then((r) => setUsers(r.data))
      .catch(() => setError('Failed to load users.'))
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const handleRoleChange = async (userId, newRole) => {
    setError(''); setMsg('');
    setSaving(userId);
    try {
      await updateUserRole(userId, newRole);
      setUsers((prev) =>
        prev.map((u) => (u.id === userId ? { ...u, role: newRole } : u))
      );
      setMsg('Role assigned.');
    } catch {
      setError('Failed to update role.');
    } finally {
      setSaving(null);
    }
  };

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );

  const pendingUsers = users.filter((u) => u.role === 'Pending');
  const activeUsers  = users.filter((u) => u.role !== 'Pending');

  return (
    <div className="container mt-4">
      <h2 className="fw-bold mb-1">User Management</h2>
      <p className="text-muted mb-4">Assign roles to registered users.</p>

      {error && <div className="alert alert-danger">{error}</div>}
      {msg   && <div className="alert alert-success">{msg}</div>}

      {/* ── Pending approvals ── */}
      {pendingUsers.length > 0 && (
        <div className="mb-5">
          <h5 className="fw-semibold text-warning mb-3">
            Pending Approval
            <span className="badge bg-warning text-dark ms-2">{pendingUsers.length}</span>
          </h5>
          <div className="table-responsive">
            <table className="table table-hover align-middle border">
              <thead className="table-warning">
                <tr>
                  <th>#</th>
                  <th>Username</th>
                  <th>Email</th>
                  <th>Assign Role</th>
                </tr>
              </thead>
              <tbody>
                {pendingUsers.map((u) => (
                  <tr key={u.id}>
                    <td>{u.id}</td>
                    <td className="fw-semibold">{u.username}</td>
                    <td>{u.email}</td>
                    <td>
                      <div className="d-flex align-items-center gap-2">
                        <select
                          className="form-select form-select-sm"
                          style={{ maxWidth: 150 }}
                          defaultValue=""
                          onChange={(e) => e.target.value && handleRoleChange(u.id, e.target.value)}
                          disabled={saving === u.id}
                        >
                          <option value="" disabled>-- Assign role --</option>
                          {ASSIGNABLE_ROLES.map((r) => (
                            <option key={r} value={r}>{r}</option>
                          ))}
                        </select>
                        {saving === u.id && (
                          <div className="spinner-border spinner-border-sm text-primary"></div>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── Active users ── */}
      <h5 className="fw-semibold mb-3">All Users</h5>
      <div className="table-responsive">
        <table className="table table-hover align-middle">
          <thead className="table-light">
            <tr>
              <th>#</th>
              <th>Username</th>
              <th>Email</th>
              <th>Role</th>
            </tr>
          </thead>
          <tbody>
            {activeUsers.map((u) => (
              <tr key={u.id}>
                <td>{u.id}</td>
                <td className="fw-semibold">
                  {u.username}
                  {u.username === currentUser?.username && (
                    <span className="badge bg-secondary ms-2">you</span>
                  )}
                </td>
                <td>{u.email}</td>
                <td>
                  {u.username === currentUser?.username ? (
                    <span className={`badge ${ROLE_BADGE[u.role] ?? 'bg-secondary'}`}>{u.role}</span>
                  ) : (
                    <div className="d-flex align-items-center gap-2">
                      <select
                        className="form-select form-select-sm"
                        style={{ maxWidth: 140 }}
                        value={u.role}
                        onChange={(e) => handleRoleChange(u.id, e.target.value)}
                        disabled={saving === u.id}
                      >
                        {ASSIGNABLE_ROLES.map((r) => (
                          <option key={r} value={r}>{r}</option>
                        ))}
                      </select>
                      {saving === u.id && (
                        <div className="spinner-border spinner-border-sm text-primary"></div>
                      )}
                    </div>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
