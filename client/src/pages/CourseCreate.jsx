import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createCourse } from '../services/courseService';
import { getAllInstructors } from '../services/instructorService';

export default function CourseCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', creditHours: 3, instructorId: '' });
  const [instructors, setInstructors] = useState([]);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getAllInstructors()
      .then((res) => setInstructors(res.data))
      .catch(() => {});
  }, []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await createCourse({ ...form, creditHours: Number(form.creditHours), instructorId: Number(form.instructorId) });
      setSuccess('Course created successfully!');
      setTimeout(() => navigate('/courses'), 1000);
    } catch (err) {
      setError(err.response?.data || 'Failed to create course.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <div className="row justify-content-center">
        <div className="col-md-6">
          <div className="d-flex align-items-center mb-4">
            <Link to="/courses" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
            <h2 className="fw-bold mb-0">Add New Course</h2>
          </div>
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Course Title</label>
                  <input
                    type="text"
                    className="form-control"
                    name="title"
                    value={form.title}
                    onChange={handleChange}
                    required
                    maxLength={200}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Credit Hours</label>
                  <input
                    type="number"
                    className="form-control"
                    name="creditHours"
                    value={form.creditHours}
                    onChange={handleChange}
                    required
                    min={1}
                    max={12}
                  />
                </div>
                <div className="mb-3">
                  <label className="form-label">Instructor</label>
                  <select
                    className="form-select"
                    name="instructorId"
                    value={form.instructorId}
                    onChange={handleChange}
                    required
                  >
                    <option value="">-- Select Instructor --</option>
                    {instructors.map((i) => (
                      <option key={i.id} value={i.id}>{i.name} ({i.department})</option>
                    ))}
                  </select>
                </div>
                <div className="d-flex gap-2">
                  <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? 'Creating...' : 'Create Course'}
                  </button>
                  <Link to="/courses" className="btn btn-outline-secondary">Cancel</Link>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
