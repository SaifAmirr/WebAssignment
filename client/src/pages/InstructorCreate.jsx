import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createInstructor } from '../services/instructorService';

export default function InstructorCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ name: '', department: '', email: '' });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await createInstructor(form);
      setSuccess('Instructor created successfully!');
      setTimeout(() => navigate('/instructors'), 1000);
    } catch (err) {
      setError(err.response?.data || 'Failed to create instructor.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <div className="row justify-content-center">
        <div className="col-md-5">
          <div className="d-flex align-items-center mb-4">
            <Link to="/instructors" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
            <h2 className="fw-bold mb-0">Add New Instructor</h2>
          </div>
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
                <div className="d-flex gap-2">
                  <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? 'Creating...' : 'Create Instructor'}
                  </button>
                  <Link to="/instructors" className="btn btn-outline-secondary">Cancel</Link>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
