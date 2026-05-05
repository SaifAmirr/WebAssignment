import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const ROLE_BADGE = { Admin: 'bg-danger', Instructor: 'bg-warning text-dark', Student: 'bg-success' };

export default function Navigation() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';
  const isStudent    = user?.role === 'Student';

  const handleLogout = () => { logout(); navigate('/login'); };

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
      <div className="container">
        <Link className="navbar-brand fw-bold fs-4" to="/">CourseManager</Link>
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
        >
          <span className="navbar-toggler-icon"></span>
        </button>
        <div className="collapse navbar-collapse" id="navbarNav">
          <ul className="navbar-nav me-auto">
            <li className="nav-item">
              <Link className="nav-link" to="/courses">Courses</Link>
            </li>

            {/* Admin only */}
            {isAdmin && <>
              <li className="nav-item"><Link className="nav-link" to="/students">Students</Link></li>
              <li className="nav-item"><Link className="nav-link" to="/instructors">Instructors</Link></li>
              <li className="nav-item"><Link className="nav-link" to="/enrollments">Enrollments</Link></li>
            </>}

            {/* Instructor only */}
            {isInstructor && <>
              <li className="nav-item"><Link className="nav-link" to="/students">Students</Link></li>
              <li className="nav-item"><Link className="nav-link" to="/enrollments">Grade Students</Link></li>
            </>}

            {/* Student only */}
            {isStudent && (
              <li className="nav-item"><Link className="nav-link" to="/enrollments">My Courses</Link></li>
            )}
          </ul>
          <ul className="navbar-nav align-items-center">
            {user ? (
              <>
                <li className="nav-item me-2">
                  <span className="nav-link text-white-50 pe-0">{user.username}</span>
                </li>
                <li className="nav-item me-2">
                  <span className={`badge ${ROLE_BADGE[user.role] ?? 'bg-secondary'}`}>
                    {user.role}
                  </span>
                </li>
                <li className="nav-item">
                  <button className="btn btn-outline-light btn-sm" onClick={handleLogout}>
                    Logout
                  </button>
                </li>
              </>
            ) : (
              <>
                <li className="nav-item">
                  <Link className="nav-link" to="/login">Login</Link>
                </li>
                <li className="nav-item">
                  <Link className="nav-link" to="/register">Register</Link>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
}
