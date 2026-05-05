import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getAllStudents } from '../services/studentService';
import { getInstructorCourses } from '../services/instructorService';
import { getAllCourses } from '../services/courseService';
import {
  getStudentEnrollments,
  getCourseEnrollments,
  updateGrade,
} from '../services/enrollmentService';
import { withdrawFromCourse } from '../services/studentService';

export default function EnrollmentsPage() {
  const { user } = useAuth();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';
  const isStudent    = user?.role === 'Student';

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="fw-bold mb-0">Enrollments</h2>
        {isAdmin && (
          <Link to="/enrollments/new" className="btn btn-primary">+ Enroll Student</Link>
        )}
      </div>

      {isStudent    && <StudentEnrollmentView user={user} />}
      {isInstructor && <InstructorEnrollmentView user={user} />}
      {isAdmin      && <AdminEnrollmentView />}
    </div>
  );
}

/* ─── Student View ───────────────────────────────────────────────── */
function StudentEnrollmentView({ user }) {
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading]         = useState(true);
  const [error, setError]             = useState('');
  const [msg, setMsg]                 = useState('');

  const load = () => {
    if (!user?.studentId) { setLoading(false); return; }
    getStudentEnrollments(user.studentId)
      .then((r) => setEnrollments(r.data))
      .catch(() => setError('Failed to load your enrollments.'))
      .finally(() => setLoading(false));
  };

  useEffect(load, [user?.studentId]);

  const handleWithdraw = async (courseId) => {
    if (!window.confirm('Withdraw from this course?')) return;
    setError(''); setMsg('');
    try {
      await withdrawFromCourse(user.studentId, courseId);
      setMsg('Withdrawn successfully.');
      load();
    } catch (err) {
      setError(err.response?.data || 'Withdrawal failed.');
    }
  };

  if (!user?.studentId) return (
    <div className="alert alert-warning">
      Your account is not linked to a student record. Contact an admin.
    </div>
  );

  if (loading) return <div className="text-center"><div className="spinner-border text-primary"></div></div>;

  return (
    <>
      <h5 className="text-muted mb-3">My Enrolled Courses</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {msg   && <div className="alert alert-success">{msg}</div>}

      {enrollments.length === 0 ? (
        <div className="alert alert-info">
          You are not enrolled in any courses.{' '}
          <Link to="/courses">Browse courses</Link> to enroll.
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>Course</th>
                <th>Enrolled On</th>
                <th>Grade</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.map((e) => (
                <tr key={e.id}>
                  <td className="fw-semibold">{e.courseName}</td>
                  <td>{new Date(e.enrollmentDate).toLocaleDateString()}</td>
                  <td>
                    <span className={`badge ${e.grade ? 'bg-primary' : 'bg-secondary'}`}>
                      {e.grade || 'In Progress'}
                    </span>
                  </td>
                  <td>
                    {!e.grade && (
                      <button
                        className="btn btn-sm btn-outline-danger"
                        onClick={() => handleWithdraw(e.courseId)}
                      >
                        Withdraw
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}

/* ─── Instructor View ────────────────────────────────────────────── */
function InstructorEnrollmentView({ user }) {
  const [courses, setCourses]         = useState([]);
  const [selectedCourse, setSelected] = useState('');
  const [enrollments, setEnrollments] = useState([]);
  const [gradeInputs, setGradeInputs] = useState({});
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState('');
  const [msg, setMsg]                 = useState('');

  useEffect(() => {
    const request = user?.instructorId
      ? getInstructorCourses(user.instructorId)
      : getAllCourses();
    request
      .then((r) => setCourses(r.data))
      .catch(() => setError('Failed to load courses.'));
  }, [user?.instructorId]);

  const loadEnrollments = async (courseId) => {
    setLoading(true); setError(''); setMsg('');
    try {
      const r = await getCourseEnrollments(courseId);
      setEnrollments(r.data);
      const inputs = {};
      r.data.forEach((e) => { inputs[e.studentId] = e.grade || ''; });
      setGradeInputs(inputs);
    } catch {
      setError('Failed to load enrollments.');
    } finally {
      setLoading(false);
    }
  };

  const handleCourseChange = (e) => {
    setSelected(e.target.value);
    if (e.target.value) loadEnrollments(e.target.value);
    else setEnrollments([]);
  };

  const handleGradeSave = async (studentId, courseId) => {
    setError(''); setMsg('');
    try {
      await updateGrade(studentId, courseId, { grade: gradeInputs[studentId] || null });
      setMsg('Grade saved.');
      loadEnrollments(courseId);
    } catch (err) {
      setError(err.response?.data || 'Failed to save grade.');
    }
  };

  return (
    <>
      <h5 className="text-muted mb-3">Manage Course Enrollments</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {msg   && <div className="alert alert-success">{msg}</div>}

      <div className="card shadow-sm border-0 mb-4">
        <div className="card-body">
          <label className="form-label fw-semibold">Select a course</label>
          <select className="form-select" value={selectedCourse} onChange={handleCourseChange}>
            <option value="">-- Select a Course --</option>
            {courses.map((c) => (
              <option key={c.id} value={c.id}>{c.title}</option>
            ))}
          </select>
        </div>
      </div>

      {loading && <div className="text-center"><div className="spinner-border text-primary"></div></div>}

      {!loading && enrollments.length > 0 && (
        <div className="table-responsive">
          <table className="table table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>Student</th>
                <th>Enrolled</th>
                <th>Current Grade</th>
                <th>Assign Grade</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.map((e) => (
                <tr key={e.id}>
                  <td className="fw-semibold">{e.studentName}</td>
                  <td>{new Date(e.enrollmentDate).toLocaleDateString()}</td>
                  <td>
                    <span className={`badge ${e.grade ? 'bg-success' : 'bg-secondary'}`}>
                      {e.grade || 'None'}
                    </span>
                  </td>
                  <td>
                    <div className="input-group input-group-sm" style={{ maxWidth: 200 }}>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="A, B+, Pass…"
                        value={gradeInputs[e.studentId] ?? ''}
                        onChange={(ev) =>
                          setGradeInputs({ ...gradeInputs, [e.studentId]: ev.target.value })
                        }
                        maxLength={4}
                      />
                      <button
                        className="btn btn-outline-success"
                        onClick={() => handleGradeSave(e.studentId, e.courseId)}
                      >
                        Save
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {!loading && selectedCourse && enrollments.length === 0 && (
        <div className="alert alert-info">No students enrolled in this course.</div>
      )}
    </>
  );
}

/* ─── Admin View ─────────────────────────────────────────────────── */
function AdminEnrollmentView() {
  const [students, setStudents]       = useState([]);
  const [courses, setCourses]         = useState([]);
  const [selectedStudent, setSelected]= useState('');
  const [enrollments, setEnrollments] = useState([]);
  const [gradeInputs, setGradeInputs] = useState({});
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState('');
  const [msg, setMsg]                 = useState('');

  useEffect(() => {
    Promise.all([getAllStudents(), getAllCourses()])
      .then(([s, c]) => { setStudents(s.data); setCourses(c.data); })
      .catch(() => setError('Failed to load data.'));
  }, []);

  const loadEnrollments = async (studentId) => {
    setLoading(true); setError(''); setMsg('');
    try {
      const r = await getStudentEnrollments(studentId);
      setEnrollments(r.data);
      const inputs = {};
      r.data.forEach((e) => { inputs[e.courseId] = e.grade || ''; });
      setGradeInputs(inputs);
    } catch {
      setError('Failed to load enrollments.');
    } finally {
      setLoading(false);
    }
  };

  const handleStudentChange = (e) => {
    setSelected(e.target.value);
    if (e.target.value) loadEnrollments(e.target.value);
    else setEnrollments([]);
  };

  const handleGradeSave = async (studentId, courseId) => {
    setError(''); setMsg('');
    try {
      await updateGrade(studentId, courseId, { grade: gradeInputs[courseId] || null });
      setMsg('Grade saved.');
      loadEnrollments(studentId);
    } catch (err) {
      setError(err.response?.data || 'Failed to save grade.');
    }
  };

  return (
    <>
      <h5 className="text-muted mb-3">View & Grade Enrollments by Student</h5>
      {error && <div className="alert alert-danger">{error}</div>}
      {msg   && <div className="alert alert-success">{msg}</div>}

      <div className="card shadow-sm border-0 mb-4">
        <div className="card-body">
          <label className="form-label fw-semibold">Select a student</label>
          <select className="form-select" value={selectedStudent} onChange={handleStudentChange}>
            <option value="">-- Select Student --</option>
            {students.map((s) => (
              <option key={s.id} value={s.id}>{s.name}</option>
            ))}
          </select>
        </div>
      </div>

      {loading && <div className="text-center"><div className="spinner-border text-primary"></div></div>}

      {!loading && enrollments.length > 0 && (
        <div className="table-responsive">
          <table className="table table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>Course</th>
                <th>Enrolled</th>
                <th>Grade</th>
                <th>Update Grade</th>
              </tr>
            </thead>
            <tbody>
              {enrollments.map((e) => (
                <tr key={e.id}>
                  <td className="fw-semibold">{e.courseName}</td>
                  <td>{new Date(e.enrollmentDate).toLocaleDateString()}</td>
                  <td>
                    <span className={`badge ${e.grade ? 'bg-success' : 'bg-secondary'}`}>
                      {e.grade || 'None'}
                    </span>
                  </td>
                  <td>
                    <div className="input-group input-group-sm" style={{ maxWidth: 200 }}>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="A, B+, Pass…"
                        value={gradeInputs[e.courseId] ?? ''}
                        onChange={(ev) =>
                          setGradeInputs({ ...gradeInputs, [e.courseId]: ev.target.value })
                        }
                        maxLength={4}
                      />
                      <button
                        className="btn btn-outline-success"
                        onClick={() => handleGradeSave(e.studentId, e.courseId)}
                      >
                        Save
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {!loading && selectedStudent && enrollments.length === 0 && (
        <div className="alert alert-info">No enrollments found for this student.</div>
      )}
    </>
  );
}
