import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { user, isAdmin, isInstructor, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">CourseManager</Link>
      </div>
      <div className="navbar-links">
        <Link to="/courses">Courses</Link>
        {(isAdmin || isInstructor) && <Link to="/students">Students</Link>}
        {user && <Link to="/instructors">Instructors</Link>}
        {isAdmin && <Link to="/enrollments">Enrollments</Link>}
      </div>
      <div className="navbar-auth">
        {user ? (
          <>
            <span className="navbar-user">{user.username} ({user.role ?? 'User'})</span>
            <button onClick={handleLogout} className="btn btn-outline">Logout</button>
          </>
        ) : (
          <>
            <Link to="/login" className="btn btn-outline">Login</Link>
            <Link to="/register" className="btn btn-primary">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}
