import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getInstructorById, updateInstructor, getInstructorCourses } from '../services/instructorService';

export default function InstructorDetail() {
  const { id } = useParams();
  const [form, setForm] = useState({ name: '', department: '', email: '' });
  const [courses, setCourses] = useState([]);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    Promise.all([
      getInstructorById(id),
      getInstructorCourses(id).catch(() => ({ data: [] })),
    ]).then(([instr, crs]) => {
      setForm({ name: instr.data.name, department: instr.data.department, email: instr.data.email });
      setCourses(crs.data);
    }).catch(() => setError('Failed to load instructor.'))
      .finally(() => setLoading(false));
  }, [id]);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSaving(true);
    try {
      await updateInstructor(id, form);
      setSuccess('Instructor updated successfully!');
    } catch (err) {
      setError(err.response?.data || 'Failed to update instructor.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div className="container mt-5 text-center"><div className="spinner-border text-primary"></div></div>;

  return (
    <div className="container mt-4">
      <div className="d-flex align-items-center mb-4">
        <Link to="/instructors" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
        <h2 className="fw-bold mb-0">Instructor Details</h2>
      </div>
      <div className="row g-4">
        <div className="col-md-5">
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Name</label>
                  <input
                    type="text"
                    className="form-control"
                    name="name"
                    value={form.name}
                    onChange={handleChange}
                    required
                    maxLength={100}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Department</label>
                  <input
                    type="text"
                    className="form-control"
                    name="department"
                    value={form.department}
                    onChange={handleChange}
                    required
                    maxLength={100}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Email</label>
                  <input
                    type="email"
                    className="form-control"
                    name="email"
                    value={form.email}
                    onChange={handleChange}
                    required
                  />
                </div>
                <button type="submit" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Saving...' : 'Update Instructor'}
                </button>
              </form>
            </div>
          </div>
        </div>

        <div className="col-md-7">
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white fw-semibold">Courses Taught</div>
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
