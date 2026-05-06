import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Navigation from "./components/Navigation";
import ProtectedRoute from "./components/ProtectedRoute";
import { AuthProvider } from "./context/AuthContext";

import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";

import CoursesPage from "./pages/CoursesPage";
import CourseCreate from "./pages/CourseCreate";
import CourseEdit from "./pages/CourseEdit";

import StudentsPage from "./pages/StudentsPage";
import StudentCreate from "./pages/StudentCreate";
import StudentDetail from "./pages/StudentDetail";

import InstructorsPage from "./pages/InstructorsPage";
import InstructorCreate from "./pages/InstructorCreate";
import InstructorDetail from "./pages/InstructorDetail";

import EnrollmentsPage from "./pages/EnrollmentsPage";
import EnrollmentCreate from "./pages/EnrollmentCreate";
import UsersPage from "./pages/UsersPage";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="bg-light min-vh-100 pb-5">
          <Navigation />
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            <Route path="/courses" element={<CoursesPage />} />
            <Route path="/courses/new" element={<ProtectedRoute roles={['Admin']}><CourseCreate /></ProtectedRoute>} />
            <Route path="/courses/:id" element={<ProtectedRoute roles={['Admin','Instructor']}><CourseEdit /></ProtectedRoute>} />

            <Route path="/students" element={<ProtectedRoute roles={['Admin','Instructor']}><StudentsPage /></ProtectedRoute>} />
            <Route path="/students/new" element={<ProtectedRoute roles={['Admin']}><StudentCreate /></ProtectedRoute>} />
            <Route path="/students/:id" element={<ProtectedRoute roles={['Admin','Instructor']}><StudentDetail /></ProtectedRoute>} />

            <Route path="/instructors" element={<ProtectedRoute roles={['Admin','Instructor']}><InstructorsPage /></ProtectedRoute>} />
            <Route path="/instructors/new" element={<ProtectedRoute roles={['Admin']}><InstructorCreate /></ProtectedRoute>} />
            <Route path="/instructors/:id" element={<ProtectedRoute roles={['Admin','Instructor']}><InstructorDetail /></ProtectedRoute>} />

            <Route path="/enrollments" element={<ProtectedRoute><EnrollmentsPage /></ProtectedRoute>} />
            <Route path="/enrollments/new" element={<ProtectedRoute roles={['Admin']}><EnrollmentCreate /></ProtectedRoute>} />

            <Route path="/users" element={<ProtectedRoute roles={['Admin']}><UsersPage /></ProtectedRoute>} />
          </Routes>
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
