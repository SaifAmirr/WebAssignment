import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getStudentById } from '../services/studentService';

export default function HomePage() {
  const { user } = useAuth();

  if (!user)                      return <GuestHome />;
  if (user.role === 'Pending')    return <PendingHome user={user} />;
  if (user.role === 'Admin')      return <AdminHome />;
  if (user.role === 'Instructor') return <InstructorHome user={user} />;
  if (user.role === 'Student')    return <StudentHome user={user} />;
  return null;
}

/* ─── Pending ─────────────────────────────────────────────────────── */
function PendingHome({ user }) {
  const { logout } = useAuth();
  return (
    <div className="container mt-5 text-center" style={{ maxWidth: 500 }}>
      <div className="card shadow-sm border-0 p-5">
        <div className="fs-1 mb-3">⏳</div>
        <h3 className="fw-bold mb-2">Waiting for approval</h3>
        <p className="text-muted mb-4">
          Hi <strong>{user.username}</strong>, your account has been created but a role has not been
          assigned yet. Please contact your admin to get access.
        </p>
        <button className="btn btn-outline-secondary btn-sm mx-auto" style={{ width: 120 }} onClick={logout}>
          Logout
        </button>
      </div>
    </div>
  );
}

/* ─── Guest ───────────────────────────────────────────────────────── */
function GuestHome() {
  return (
    <div className="container mt-5 text-center">
      <h1 className="display-4 fw-bold text-primary mb-3">Course Management System</h1>
      <p className="lead text-muted mb-4">
        Browse available courses or log in to enroll, manage students, and assign grades.
      </p>
      <div className="d-flex justify-content-center gap-2 mb-5">
        <Link to="/login"    className="btn btn-primary px-4">Login</Link>
        <Link to="/register" className="btn btn-outline-primary px-4">Register</Link>
      </div>
      <hr />
      <p className="text-muted mt-4">
        <Link to="/courses">Browse all courses →</Link>
      </p>
    </div>
  );
}

/* ─── Admin ───────────────────────────────────────────────────────── */
function AdminHome() {
  const cards = [
    { to: '/courses',     icon: '📚', title: 'Courses',     desc: 'Add, edit or delete courses and assign instructors.' },
    { to: '/students',    icon: '🎓', title: 'Students',    desc: 'Create student records and track their GPA.' },
    { to: '/instructors', icon: '👨‍🏫', title: 'Instructors', desc: 'Add instructors and manage their profiles.' },
    { to: '/enrollments', icon: '📋', title: 'Enrollments', desc: 'Enroll students into courses and update grades.' },
    { to: '/users',       icon: '🔑', title: 'Users',       desc: 'Assign roles to registered users.' },
  ];
  return (
    <div className="container mt-5">
      <h2 className="fw-bold mb-1">Admin Dashboard</h2>
      <p className="text-muted mb-4">Manage the entire system from here.</p>
      <div className="row g-4">
        {cards.map((c) => (
          <div key={c.to} className="col-md-4">
            <div className="card h-100 shadow-sm border-0 text-center p-3">
              <div className="fs-1 mb-2">{c.icon}</div>
              <h5 className="fw-semibold">{c.title}</h5>
              <p className="text-muted small">{c.desc}</p>
              <Link to={c.to} className="btn btn-primary btn-sm mt-auto">Go to {c.title}</Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

/* ─── Instructor ──────────────────────────────────────────────────── */
function InstructorHome({ user }) {
  return (
    <div className="container mt-5">
      <h2 className="fw-bold mb-1">Welcome, {user.username}</h2>
      <p className="text-muted mb-4">Here is what you can do as an Instructor.</p>

      {!user.instructorId && (
        <div className="alert alert-warning mb-4">
          Your account has not been linked to an instructor record yet. Contact your admin to complete the setup.
        </div>
      )}

      <div className="row g-4 justify-content-center">
        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0 text-center p-4">
            <div className="fs-1 mb-2">📚</div>
            <h5 className="fw-semibold">Courses</h5>
            <p className="text-muted small">
              View all available courses. Edit the title or credit hours of any course you teach.
            </p>
            <Link to="/courses" className="btn btn-primary btn-sm mt-auto">View Courses</Link>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0 text-center p-4">
            <div className="fs-1 mb-2">📝</div>
            <h5 className="fw-semibold">Grade Students</h5>
            <p className="text-muted small">
              Select one of your assigned courses to see enrolled students and assign final grades.
            </p>
            <Link to="/enrollments" className="btn btn-primary btn-sm mt-auto">Grade Students</Link>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card h-100 shadow-sm border-0 text-center p-4">
            <div className="fs-1 mb-2">🎓</div>
            <h5 className="fw-semibold">Students</h5>
            <p className="text-muted small">
              Look up any student record to view their name and GPA.
            </p>
            <Link to="/students" className="btn btn-primary btn-sm mt-auto">View Students</Link>
          </div>
        </div>
      </div>
    </div>
  );
}

/* ─── Student ─────────────────────────────────────────────────────── */
function StudentHome({ user }) {
  const { linkStudentId } = useAuth();
  const [form, setForm]       = useState({ firstName: '', lastName: '', studentId: '' });
  const [linkError, setLinkError] = useState('');
  const [linking, setLinking] = useState(false);
  const [linked, setLinked]   = useState(false);

  const handleChange = (e) => {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
    setLinkError('');
  };

  const handleLink = async () => {
    const { firstName, lastName, studentId } = form;
    if (!firstName.trim() || !lastName.trim()) {
      setLinkError('Please enter both your first and last name.');
      return;
    }
    if (!studentId || isNaN(Number(studentId)) || Number(studentId) < 1) {
      setLinkError('Please enter a valid Student ID.');
      return;
    }
    setLinking(true);
    setLinkError('');
    try {
      const res = await getStudentById(Number(studentId));
      const student = res.data;
      const fullName = `${firstName.trim()} ${lastName.trim()}`.toLowerCase();
      if (student.name?.toLowerCase() !== fullName) {
        setLinkError('Name does not match the student record. Please check your name and ID.');
        return;
      }
      linkStudentId(Number(studentId));
      setLinked(true);
    } catch {
      setLinkError('Student ID not found. Please check your ID and try again.');
    } finally {
      setLinking(false);
    }
  };

  const isLinked = user.studentId || linked;

  return (
    <div className="container mt-5">
      <h2 className="fw-bold mb-1">Welcome, {user.username}</h2>
      <p className="text-muted mb-4">Here is what you can do as a Student.</p>

      {/* ── Link prompt: shown until studentId is known ── */}
      {!isLinked && (
        <div className="card border-warning shadow-sm mb-4">
          <div className="card-body">
            <h6 className="fw-bold mb-1 text-warning">One more step before you can enroll</h6>
            <p className="text-muted small mb-3">
              Enter your name and Student ID exactly as your admin registered you.
              Your admin can find your Student ID in the <strong>Students</strong> list.
            </p>
            <div className="row g-2">
              <div className="col-sm-4">
                <input
                  type="text"
                  className="form-control"
                  placeholder="First name"
                  name="firstName"
                  value={form.firstName}
                  onChange={handleChange}
                />
              </div>
              <div className="col-sm-4">
                <input
                  type="text"
                  className="form-control"
                  placeholder="Last name"
                  name="lastName"
                  value={form.lastName}
                  onChange={handleChange}
                />
              </div>
              <div className="col-sm-2">
                <input
                  type="number"
                  className="form-control"
                  placeholder="Student ID"
                  name="studentId"
                  value={form.studentId}
                  onChange={handleChange}
                  min={1}
                />
              </div>
              <div className="col-sm-2">
                <button className="btn btn-warning w-100 fw-semibold" onClick={handleLink} disabled={linking}>
                  {linking ? 'Checking…' : 'Confirm'}
                </button>
              </div>
            </div>
            {linkError && <div className="text-danger small mt-2">{linkError}</div>}
          </div>
        </div>
      )}

      {isLinked && (
        <div className="alert alert-success mb-4">
          Your student record is linked. You can now enroll in courses.
        </div>
      )}

      <div className="row g-4 justify-content-center">
        <div className="col-md-5">
          <div className="card h-100 shadow-sm border-0 text-center p-4">
            <div className="fs-1 mb-2">📚</div>
            <h5 className="fw-semibold">Browse Courses</h5>
            <p className="text-muted small">
              See all available courses, their credit hours, and assigned instructors.
              Your admin will enroll you once you decide which courses you want.
            </p>
            <Link to="/courses" className="btn btn-primary btn-sm mt-auto">Browse Courses</Link>
          </div>
        </div>
        <div className="col-md-5">
          <div className="card h-100 shadow-sm border-0 text-center p-4">
            <div className="fs-1 mb-2">🎓</div>
            <h5 className="fw-semibold">My Courses</h5>
            <p className="text-muted small">
              View the courses you are enrolled in, check your grades, and withdraw from a course if needed.
            </p>
            <Link to="/enrollments" className="btn btn-primary btn-sm mt-auto">My Courses</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
