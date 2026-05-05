import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createEnrollment } from '../services/enrollmentService';
import { getAllStudents } from '../services/studentService';
import { getAllCourses } from '../services/courseService';
import { useAuth } from '../context/AuthContext';

export default function EnrollmentCreate() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';

  const [form, setForm] = useState({
    studentId: isAdmin ? '' : (user?.studentId ?? ''),
    courseId: '',
  });
  const [students, setStudents] = useState([]);
  const [courses, setCourses]   = useState([]);
  const [error, setError]       = useState('');
  const [success, setSuccess]   = useState('');
  const [loading, setLoading]   = useState(false);

  useEffect(() => {
    const requests = [getAllCourses()];
    if (isAdmin) requests.push(getAllStudents());
    Promise.all(requests)
      .then(([c, s]) => {
        setCourses(c.data);
        if (s) setStudents(s.data);
      })
      .catch(() => setError('Failed to load data.'));
  }, [isAdmin]);

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await createEnrollment({
        studentId: Number(form.studentId),
        courseId:  Number(form.courseId),
      });
      setSuccess('Enrollment created successfully!');
      setTimeout(() => navigate('/enrollments'), 1000);
    } catch (err) {
      setError(err.response?.data || 'Failed to create enrollment.');
    } finally {
      setLoading(false);
    }
  };

  if (!isAdmin && !user?.studentId) return (
    <div className="container mt-4">
      <div className="alert alert-warning">
        Your account is not linked to a student record. Ask an admin to link your account before enrolling.
      </div>
      <Link to="/enrollments" className="btn btn-outline-secondary">← Back</Link>
    </div>
  );

  return (
    <div className="container mt-4">
      <div className="row justify-content-center">
        <div className="col-md-5">
          <div className="d-flex align-items-center mb-4">
            <Link to="/enrollments" className="btn btn-outline-secondary btn-sm me-3">← Back</Link>
            <h2 className="fw-bold mb-0">
              {isAdmin ? 'Enroll a Student' : 'Enroll in a Course'}
            </h2>
          </div>
          <div className="card shadow-sm border-0">
            <div className="card-body p-4">
              {error   && <div className="alert alert-danger">{error}</div>}
              {success && <div className="alert alert-success">{success}</div>}
              <form onSubmit={handleSubmit}>
                {isAdmin ? (
                  <div className="mb-3">
                    <label className="form-label">Student</label>
                    <select
                      className="form-select"
                      name="studentId"
                      value={form.studentId}
                      onChange={handleChange}
                      required
                    >
                      <option value="">-- Select Student --</option>
                      {students.map((s) => (
                        <option key={s.id} value={s.id}>{s.name} (GPA {s.gpa.toFixed(2)})</option>
                      ))}
                    </select>
                  </div>
                ) : (
                  <div className="mb-3">
                    <label className="form-label">Enrolling as</label>
                    <input
                      className="form-control"
                      value={`Student ID: ${user.studentId}`}
                      disabled
                    />
                  </div>
                )}

                <div className="mb-3">
                  <label className="form-label">Course</label>
                  <select
                    className="form-select"
                    name="courseId"
                    value={form.courseId}
                    onChange={handleChange}
                    required
                  >
                    <option value="">-- Select Course --</option>
                    {courses.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.title} ({c.creditHours} cr){c.instructorName ? ` — ${c.instructorName}` : ''}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="d-flex gap-2">
                  <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? 'Enrolling...' : 'Confirm Enrollment'}
                  </button>
                  <Link to="/enrollments" className="btn btn-outline-secondary">Cancel</Link>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
