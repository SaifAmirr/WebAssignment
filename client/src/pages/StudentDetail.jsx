import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getStudentById, updateStudent } from '../services/studentService';
import { getStudentEnrollments } from '../services/enrollmentService';
import { useAuth } from '../context/AuthContext';

export default function StudentDetail() {
  const { id } = useParams();
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';

  const [form, setForm]           = useState({ name: '', gpa: 0 });
  const [enrollments, setEnrollments] = useState([]);
  const [error, setError]         = useState('');
  const [success, setSuccess]     = useState('');
  const [loading, setLoading]     = useState(true);
  const [saving, setSaving]       = useState(false);

  useEffect(() => {
    Promise.all([
      getStudentById(id),
      getStudentEnrollments(id).catch(() => ({ data: [] })),
    ]).then(([student, enroll]) => {
      setForm({ name: student.data.name, gpa: student.data.gpa });
      setEnrollments(enroll.data);
    }).catch(() => setError('Failed to load student.'))
      .finally(() => setLoading(false));
  }, [id]);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSaving(true);
    try {
      await updateStudent(id, { name: form.name, gpa: Number(form.gpa) });
      setSuccess('Student updated successfully!');
    } catch (err) {
      setError(err.response?.data || 'Failed to update student.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );

  return (
    <div className="container mt-4">
      <div className="d-flex align-items-center mb-4">
        <Link to="/students" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
        <h2 className="fw-bold mb-0">Student Details</h2>
      </div>
      <div className="row g-4">
        <div className="col-md-5">
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error   && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}

              {isAdmin ? (
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
                  </div>
                  <button type="submit" className="btn btn-primary" disabled={saving}>
                    {saving ? 'Saving...' : 'Update Student'}
                  </button>
                </form>
              ) : (
                <dl className="row mb-0">
                  <dt className="col-sm-4">Name</dt>
                  <dd className="col-sm-8">{form.name}</dd>
                  <dt className="col-sm-4">GPA</dt>
                  <dd className="col-sm-8">
                    <span className={`badge ${form.gpa >= 3.5 ? 'bg-success' : form.gpa >= 2.0 ? 'bg-warning text-dark' : 'bg-danger'}`}>
                      {Number(form.gpa).toFixed(2)}
                    </span>
                  </dd>
                </dl>
              )}
            </div>
          </div>
        </div>

        <div className="col-md-7">
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white fw-semibold">Enrolled Courses</div>
            <div className="card-body p-0">
              {enrollments.length === 0 ? (
                <p className="p-3 text-muted mb-0">No enrollments found.</p>
              ) : (
                <ul className="list-group list-group-flush">
                  {enrollments.map((e) => (
                    <li key={e.id} className="list-group-item d-flex justify-content-between align-items-center">
                      <div>
                        <span className="fw-semibold">{e.courseName}</span>
                        <small className="text-muted ms-2">{new Date(e.enrollmentDate).toLocaleDateString()}</small>
                      </div>
                      <span className={`badge ${e.grade ? 'bg-primary' : 'bg-secondary'}`}>
                        {e.grade || 'In progress'}
                      </span>
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
