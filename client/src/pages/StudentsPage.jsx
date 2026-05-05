import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllStudents } from '../services/studentService';
import { useAuth } from '../context/AuthContext';

export default function StudentsPage() {
  const { user } = useAuth();
  const isAdmin      = user?.role === 'Admin';
  const isInstructor = user?.role === 'Instructor';

  const [students, setStudents] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');

  useEffect(() => {
    getAllStudents()
      .then((res) => setStudents(res.data))
      .catch(() => setError('Failed to load students.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return (
    <div className="container mt-5 text-center">
      <div className="spinner-border text-primary"></div>
    </div>
  );

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
        <div className="table-responsive">
          <table className="table table-hover align-middle">
            <thead className="table-light">
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>GPA</th>
                {(isAdmin || isInstructor) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((s) => (
                <tr key={s.id}>
                  <td>{s.id}</td>
                  <td className="fw-semibold">{s.name}</td>
                  <td>
                    <span className={`badge ${s.gpa >= 3.5 ? 'bg-success' : s.gpa >= 2.0 ? 'bg-warning text-dark' : 'bg-danger'}`}>
                      {s.gpa.toFixed(2)}
                    </span>
                  </td>
                  {(isAdmin || isInstructor) && (
                    <td>
                      <Link to={`/students/${s.id}`} className="btn btn-sm btn-outline-primary">
                        {isAdmin ? 'View / Edit' : 'View'}
                      </Link>
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
