import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getInstructorById, updateInstructor, deleteInstructor, getInstructorCourses } from '../services/instructorService';
import { getAllUsers } from '../services/authService';
import { useAuth } from '../context/AuthContext';

export default function InstructorDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user: currentUser } = useAuth();
  const isAdmin = currentUser?.role === 'Admin';

  const [form, setForm]       = useState({ name: '', department: '', email: '' });
  const [courses, setCourses] = useState([]);
  const [users, setUsers]     = useState([]);
  const [linkedUserId, setLinkedUserId]   = useState('');
  const [selectedUserId, setSelectedUserId] = useState('');
  const [error, setError]     = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving]   = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetches = [
      getInstructorById(id),
      getInstructorCourses(id).catch(() => ({ data: [] })),
    ];
    if (isAdmin) fetches.push(getAllUsers().catch(() => ({ data: [] })));

    Promise.all(fetches).then(([instr, crs, usersRes]) => {
      setForm({ name: instr.data.name, department: instr.data.department, email: instr.data.email });
      setCourses(crs.data);
      if (usersRes) {
        const instructorUsers = usersRes.data.filter((u) => u.role === 'Instructor');
        setUsers(instructorUsers);
        const linked = usersRes.data.find((u) => u.instructorId === Number(id));
        if (linked) { setLinkedUserId(linked.id); setSelectedUserId(String(linked.id)); }
      }
    }).catch(() => setError('Failed to load instructor.'))
      .finally(() => setLoading(false));
  }, [id, isAdmin]);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setSuccess('');
    setSaving(true);
    try {
      const payload = { ...form };
      if (isAdmin && selectedUserId && Number(selectedUserId) !== linkedUserId) {
        payload.userId = Number(selectedUserId);
      }
      await updateInstructor(id, payload);
      if (isAdmin && selectedUserId && Number(selectedUserId) !== linkedUserId) {
        setLinkedUserId(Number(selectedUserId));
      }
      setSuccess('Instructor updated successfully!');
    } catch (err) {
      setError(err.response?.data || 'Failed to update instructor.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Delete this instructor? This cannot be undone.')) return;
    setDeleting(true);
    try {
      await deleteInstructor(id);
      navigate('/instructors');
    } catch (err) {
      setError(err.response?.data || 'Failed to delete instructor.');
      setDeleting(false);
    }
  };

  if (loading) return <div className="container mt-5 text-center"><div className="spinner-border text-primary"></div></div>;

  const currentLinkedUser = users.find((u) => u.id === linkedUserId);

  return (
    <div className="container mt-4">
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div className="d-flex align-items-center">
          <Link to="/instructors" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
          <h2 className="fw-bold mb-0">Instructor Details</h2>
        </div>
        {isAdmin && (
          <button className="btn btn-danger btn-sm" onClick={handleDelete} disabled={deleting}>
            {deleting ? 'Deleting…' : 'Delete Instructor'}
          </button>
        )}
      </div>

      <div className="row g-4">
        {/* ── Left: edit form ── */}
        <div className="col-md-5">
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error   && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Name</label>
                  <input type="text" className="form-control" name="name"
                    value={form.name} onChange={handleChange} required maxLength={100} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Department</label>
                  <input type="text" className="form-control" name="department"
                    value={form.department} onChange={handleChange} required maxLength={100} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Email</label>
                  <input type="email" className="form-control" name="email"
                    value={form.email} onChange={handleChange} required />
                </div>

                {/* ── Link to user account (Admin only) ── */}
                {isAdmin && (
                  <div className="mb-3">
                    <label className="form-label">Linked user account</label>
                    {currentLinkedUser && (
                      <div className="mb-1">
                        <span className="badge bg-success me-1">Currently linked</span>
                        <span className="text-muted small">{currentLinkedUser.username} ({currentLinkedUser.email})</span>
                      </div>
                    )}
                    <select
                      className="form-select"
                      value={selectedUserId}
                      onChange={(e) => setSelectedUserId(e.target.value)}
                    >
                      <option value="">-- No user linked --</option>
                      {users.map((u) => (
                        <option key={u.id} value={u.id}>
                          {u.username} ({u.email}){u.instructorId && u.id !== linkedUserId ? ' — already linked' : ''}
                        </option>
                      ))}
                    </select>
                    <div className="form-text">Changing this will link the selected account to this instructor record.</div>
                  </div>
                )}

                <button type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Saving…' : 'Save Changes'}
                </button>
              </form>
            </div>
          </div>
        </div>

        {/* ── Right: courses ── */}
        <div className="col-md-7">
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white fw-semibold">
              Courses Taught ({courses.length})
            </div>
            <div className="card-body p-0">
              {courses.length === 0 ? (
                <p className="p-3 text-muted mb-0">No courses assigned.</p>
              ) : (
                <ul className="list-group list-group-flush">
                  {courses.map((c) => (
                    <li key={c.id} className="list-group-item d-flex justify-content-between align-items-center">
                      <span className="fw-semibold">{c.title}</span>
                      <span className="badge bg-primary">{c.creditHours} cr</span>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
