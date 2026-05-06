import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllStudents } from '../services/studentService';
import { getInstructorCourses } from '../services/instructorService';
import { getCourseEnrollments } from '../services/enrollmentService';
import { useAuth } from '../context/AuthContext';

export default function StudentsPage() {
  const { user } = useAuth();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';

  return isInstructor
    ? <InstructorStudentView user={user} />
    : <AdminStudentView isAdmin={isAdmin} />;
}

/* ─── Admin view: flat list of all students ─────────────────────── */
function AdminStudentView({ isAdmin }) {
  const [students, setStudents] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');

  useEffect(() => {
    getAllStudents()
      .then((res) => setStudents(res.data))
      .catch(() => setError('Failed to load students.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Spinner />;

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="fw-bold mb-0">Students</h2>
        {isAdmin && <Link to="/students/new" className="btn btn-primary">+ Add Student</Link>}
      </div>
      {error && <div className="alert alert-danger">{error}</div>}
      {students.length === 0 ? (
        <div className="alert alert-info">No students found.</div>
      ) : (
        <StudentTable students={students} showGpa={isAdmin} isAdmin={isAdmin} />
      )}
    </div>
  );
}

/* ─── Instructor view: per-course tabs ──────────────────────────── */
function InstructorStudentView({ user }) {
  const [courses, setCourses]             = useState([]);
  const [enrollments, setEnrollments]     = useState({});   // courseId → [{studentId, studentName, grade}]
  const [activeCourse, setActiveCourse]   = useState(null);
  const [loadingCourses, setLoadingCourses] = useState(true);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [error, setError]                 = useState('');

  useEffect(() => {
    if (!user?.instructorId) return;
    getInstructorCourses(user.instructorId)
      .then((res) => {
        setCourses(res.data);
        if (res.data.length > 0) setActiveCourse(res.data[0].id);
      })
      .catch(() => setError('Failed to load your courses.'))
      .finally(() => setLoadingCourses(false));
  }, [user?.instructorId]);

  useEffect(() => {
    if (!activeCourse) return;
    if (enrollments[activeCourse]) return;
    setLoadingStudents(true);
    getCourseEnrollments(activeCourse)
      .then((res) => setEnrollments((prev) => ({ ...prev, [activeCourse]: res.data })))
      .catch(() => setError('Failed to load students for this course.'))
      .finally(() => setLoadingStudents(false));
  }, [activeCourse]);

  if (!user?.instructorId) return (
    <div className="container mt-4">
      <div className="alert alert-warning">
        Your account is not linked to an instructor record. Contact your admin.
      </div>
    </div>
  );

  if (loadingCourses) return <Spinner />;

  const courseStudents = enrollments[activeCourse] ?? [];
  const activeCourseTitle = courses.find((c) => c.id === activeCourse)?.title ?? '';

  return (
    <div className="container mt-4">
      <h2 className="fw-bold mb-1">My Students</h2>
      <p className="text-muted mb-4">Select a course to see its enrolled students.</p>

      {error && <div className="alert alert-danger">{error}</div>}

      {courses.length === 0 ? (
        <div className="alert alert-info">No courses have been assigned to you yet.</div>
      ) : (
        <>
          {/* Course tabs */}
          <ul className="nav nav-tabs mb-4">
            {courses.map((c) => (
              <li className="nav-item" key={c.id}>
                <button
                  className={`nav-link ${activeCourse === c.id ? 'active fw-semibold' : ''}`}
                  onClick={() => setActiveCourse(c.id)}
                >
                  {c.title}
                </button>
              </li>
            ))}
          </ul>

          {/* Student table for active course */}
          {loadingStudents ? (
            <Spinner />
          ) : courseStudents.length === 0 ? (
            <div className="alert alert-info">
              No students enrolled in <strong>{activeCourseTitle}</strong>.
            </div>
          ) : (
            <>
              <p className="text-muted mb-3">
                <strong>{courseStudents.length}</strong> student{courseStudents.length !== 1 ? 's' : ''} enrolled in <strong>{activeCourseTitle}</strong>
              </p>
              <div className="table-responsive">
                <table className="table table-hover align-middle">
                  <thead className="table-light">
                    <tr>
                      <th>#</th>
                      <th>Name</th>
                      <th>Grade</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {courseStudents.map((e) => (
                      <tr key={e.studentId}>
                        <td>{e.studentId}</td>
                        <td className="fw-semibold">{e.studentName}</td>
                        <td>
                          <span className={`badge ${e.grade ? 'bg-success' : 'bg-secondary'}`}>
                            {e.grade || 'No grade yet'}
                          </span>
                        </td>
                        <td>
                          <Link to={`/students/${e.studentId}`} className="btn btn-sm btn-outline-primary">
                            View
                          </Link>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </>
      )}
    </div>
  );
}

/* ─── Shared helpers ────────────────────────────────────────────── */
function StudentTable({ students, showGpa, isAdmin }) {
  return (
    <div className="table-responsive">
      <table className="table table-hover align-middle">
        <thead className="table-light">
          <tr>
            <th>Student No.</th>
            <th>Name</th>
            {showGpa && <th>GPA</th>}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {students.map((s) => (
            <tr key={s.id}>
              <td className="fw-semibold text-primary">{s.studentNumber ?? '—'}</td>
              <td className="fw-semibold">{s.name}</td>
              {showGpa && (
                <td>
                  <span className={`badge ${s.gpa >= 3.5 ? 'bg-success' : s.gpa >= 2.0 ? 'bg-warning text-dark' : 'bg-danger'}`}>
                    {s.gpa?.toFixed(2)}
                  </span>
                </td>
              )}
              <td>
                <Link to={`/students/${s.id}`} className="btn btn-sm btn-outline-primary">
                  {isAdmin ? 'View / Edit' : 'View'}
                </Link>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function Spinner() {
  return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );
}
