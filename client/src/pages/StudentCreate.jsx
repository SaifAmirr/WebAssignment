import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createStudent } from '../services/studentService';

export default function StudentCreate() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ studentNumber: '', name: '', gpa: 0.0 });
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
      await createStudent({ studentNumber: Number(form.studentNumber), name: form.name, gpa: Number(form.gpa) });
      setSuccess('Student created successfully!');
      setTimeout(() => navigate('/students'), 1000);
    } catch (err) {
      setError(err.response?.data || 'Failed to create student.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-4">
      <div className="row justify-content-center">
        <div className="col-md-5">
          <div className="d-flex align-items-center mb-4">
            <Link to="/students" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
            <h2 className="fw-bold mb-0">Add New Student</h2>
          </div>
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Student Number</label>
                  <input
                    type="number"
                    className="form-control"
                    name="studentNumber"
                    value={form.studentNumber}
                    onChange={handleChange}
                    required
                    min={1}
                    placeholder="e.g. 20231234"
                  />
                  <div className="form-text">A unique number the student will use to link their account.</div>
                </div>
                <div className="mb-3">
                  <label className="form-label">Student Name</label>
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
                  <label className="form-label">GPA</label>
                  <input
                    type="number"
                    className="form-control"
                    name="gpa"
                    value={form.gpa}
                    onChange={handleChange}
                    required
                    min={0}
                    max={4}
                    step={0.01}
                  />
                  <div className="form-text">Between 0.0 and 4.0</div>
                </div>
                <div className="d-flex gap-2">
                  <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? 'Creating...' : 'Create Student'}
                  </button>
                  <Link to="/students" className="btn btn-outline-secondary">Cancel</Link>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
