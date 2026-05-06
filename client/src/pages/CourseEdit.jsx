import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getCourseById, updateCourse } from '../services/courseService';
import { getAllInstructors } from '../services/instructorService';
import { getCourseEnrollments } from '../services/enrollmentService';
import { useAuth } from '../context/AuthContext';

export default function CourseEdit() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';

  const [form, setForm]             = useState({ title: '', creditHours: 1, instructorId: '' });
  const [instructors, setInstructors] = useState([]);
  const [enrollments, setEnrollments] = useState([]);
  const [error, setError]           = useState('');
  const [success, setSuccess]       = useState('');
  const [loading, setLoading]       = useState(true);
  const [saving, setSaving]         = useState(false);

  useEffect(() => {
    Promise.all([
      getCourseById(id),
      getAllInstructors().catch(() => ({ data: [] })),
      getCourseEnrollments(id).catch(() => ({ data: [] })),
    ]).then(([course, instr, enroll]) => {
      const c = course.data;
      setForm({ title: c.title, creditHours: c.creditHours, instructorId: c.instructorId || '' });
      setInstructors(instr.data);
      setEnrollments(enroll.data);
    }).catch(() => setError('Failed to load course.'))
      .finally(() => setLoading(false));
  }, [id]);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSaving(true);
    try {
      await updateCourse(id, {
        ...form,
        creditHours: Number(form.creditHours),
        instructorId: Number(form.instructorId),
      });
      setSuccess('Course updated successfully!');
      setTimeout(() => navigate('/courses'), 1000);
    } catch (err) {
      setError(err.response?.data || 'Failed to update course.');
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
        <Link to="/courses" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
        <h2 className="fw-bold mb-0">{isAdmin ? 'Edit Course' : 'Course Details'}</h2>
      </div>
      <div className="row g-4">
        <div className="col-md-6">
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error   && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}

              {isAdmin ? (
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
                    {isAdmin ? (
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
                    ) : (
                      <input
                        className="form-control"
                        value={instructors.find((i) => i.id === Number(form.instructorId))?.name || 'Unassigned'}
                        disabled
                      />
                    )}
                  </div>
                  <div className="d-flex gap-2">
                    <button type="submit" className="btn btn-primary" disabled={saving}>
                      {saving ? 'Saving...' : 'Save Changes'}
                    </button>
                    <Link to="/courses" className="btn btn-outline-secondary">Cancel</Link>
                  </div>
                </form>
              ) : (
                <dl className="row mb-0">
                  <dt className="col-sm-4">Title</dt>
                  <dd className="col-sm-8">{form.title}</dd>
                  <dt className="col-sm-4">Credit Hours</dt>
                  <dd className="col-sm-8">{form.creditHours}</dd>
                  <dt className="col-sm-4">Instructor</dt>
                  <dd className="col-sm-8">
                    {instructors.find((i) => i.id === Number(form.instructorId))?.name || 'Unassigned'}
                  </dd>
                </dl>
              )}
            </div>
          </div>
        </div>

        <div className="col-md-6">
          <div className="card shadow-sm border-0">
            <div className="card-header bg-white fw-semibold">
              Enrolled Students ({enrollments.length})
            </div>
            <div className="card-body p-0">
              {enrollments.length === 0 ? (
                <p className="p-3 text-muted mb-0">No students enrolled.</p>
              ) : (
                <ul className="list-group list-group-flush">
                  {enrollments.map((e) => (
                    <li key={e.id} className="list-group-item d-flex justify-content-between">
                      <span>{e.studentName}</span>
                      <span className={`badge ${e.grade ? 'bg-primary' : 'bg-secondary'}`}>
                        {e.grade || 'No grade'}
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
