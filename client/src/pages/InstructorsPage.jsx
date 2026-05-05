import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllInstructors } from '../services/instructorService';
import { useAuth } from '../context/AuthContext';

export default function InstructorsPage() {
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';

  const [instructors, setInstructors] = useState([]);
  const [loading, setLoading]         = useState(true);
  const [error, setError]             = useState('');

  useEffect(() => {
    getAllInstructors()
      .then((res) => setInstructors(res.data))
      .catch(() => setError('Failed to load instructors.'))
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
        <h2 className="fw-bold mb-0">Instructors</h2>
        {isAdmin && <Link to="/instructors/new" className="btn btn-primary">+ Add Instructor</Link>}
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      {instructors.length === 0 ? (
        <div className="alert alert-info">No instructors found.</div>
      ) : (
        <div className="row g-3">
          {instructors.map((i) => (
            <div key={i.id} className="col-md-4">
              <div className="card h-100 shadow-sm border-0">
                <div className="card-body">
                  <h5 className="card-title fw-semibold">{i.name}</h5>
                  <p className="card-text text-muted mb-1">{i.department}</p>
                  <p className="card-text">
                    <small><a href={`mailto:${i.email}`}>{i.email}</a></small>
                  </p>
                  <Link to={`/instructors/${i.id}`} className="btn btn-sm btn-outline-primary">
                    {isAdmin ? 'View / Edit' : 'View'}
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
