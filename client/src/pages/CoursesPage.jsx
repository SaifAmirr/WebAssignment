import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllCourses, deleteCourse } from '../services/courseService';
import { getInstructorCourses } from '../services/instructorService';
import { useAuth } from '../context/AuthContext';

export default function CoursesPage() {
  const { user } = useAuth();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';

  const [courses, setCourses]     = useState([]);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState('');
  const [actionMsg, setActionMsg] = useState('');

  useEffect(() => {
    if (isInstructor) {
      if (!user?.instructorId) { setLoading(false); return; }
      getInstructorCourses(user.instructorId)
        .then((res) => setCourses(res.data))
        .catch(() => setError('Failed to load your courses.'))
        .finally(() => setLoading(false));
    } else {
      getAllCourses()
        .then((res) => setCourses(res.data))
        .catch(() => setError('Failed to load courses.'))
        .finally(() => setLoading(false));
    }
  }, [isInstructor, user?.instructorId]);

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this course?')) return;
    try {
      await deleteCourse(id);
      setCourses((prev) => prev.filter((c) => c.id !== id));
      setActionMsg('Course deleted.');
    } catch (err) {
      setError(err.response?.data || 'Failed to delete course.');
    }
  };

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );

  if (isInstructor && !user?.instructorId) return (
    <div className="container mt-4">
      <div className="alert alert-warning">
        Your account is not linked to an instructor record. Contact your admin.
      </div>
    </div>
  );

  return (
    <div className="container mt-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="fw-bold mb-0">{isInstructor ? 'My Courses' : 'Courses'}</h2>
        {isAdmin && <Link to="/courses/new" className="btn btn-primary">+ Add Course</Link>}
      </div>

      {error     && <div className="alert alert-danger alert-dismissible">
        {error}<button type="button" className="btn-close" onClick={() => setError('')}></button>
      </div>}
      {actionMsg && <div className="alert alert-success alert-dismissible">
        {actionMsg}<button type="button" className="btn-close" onClick={() => setActionMsg('')}></button>
      </div>}

      {courses.length === 0 ? (
        <div className="alert alert-info">
          {isInstructor ? 'No courses have been assigned to you yet.' : 'No courses available.'}
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>#</th>
                <th>Title</th>
                <th>Credit Hours</th>
                {!isInstructor && <th>Instructor</th>}
                {isAdmin && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {courses.map((course) => (
                <tr key={course.id}>
                  <td>{course.id}</td>
                  <td className="fw-semibold">{course.title}</td>
                  <td>{course.creditHours}</td>
                  {!isInstructor && (
                    <td>{course.instructorName || <span className="text-muted">Unassigned</span>}</td>
                  )}
                  {isAdmin && (
                    <td>
                      <Link to={`/courses/${course.id}`} className="btn btn-sm btn-outline-primary me-2">
                        Edit
                      </Link>
                      <button
                        className="btn btn-sm btn-outline-danger"
                        onClick={() => handleDelete(course.id)}
                      >
                        Delete
                      </button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
