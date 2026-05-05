# Course Management System

A full-stack course management web application with a React frontend and an ASP.NET Core backend API. Supports role-based access for Admins, Instructors, and Students to manage courses, enrollments, and grades.

---

## Table of Contents

- [Application Overview](#application-overview)
- [Technologies Used](#technologies-used)
- [Setup Instructions](#setup-instructions)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Role-Based Access](#role-based-access)
- [Frontend Pages & Routes](#frontend-pages--routes)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Authentication & Security](#authentication--security)

---

## Application Overview

This application simulates a university course registration system. It covers the full lifecycle of a semester:

- **Admins** set up the system — creating courses, students, and instructors, then enrolling students into courses.
- **Instructors** manage their courses and assign final grades at the end of the semester.
- **Students** browse available courses, view their enrolled courses, check their grades, and withdraw from courses.

The frontend communicates with the backend over a REST API using JWT bearer tokens for authentication.

---

## Technologies Used

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core 10 | ORM and database migrations |
| PostgreSQL 13+ | Relational database |
| JWT Bearer | Stateless authentication |
| BCrypt.Net | Password hashing |

### Frontend
| Technology | Purpose |
|---|---|
| React 19 | UI framework |
| Vite | Build tool and dev server |
| React Router v7 | Client-side routing |
| Axios | HTTP client for API calls |
| Bootstrap 5 | Styling and layout |

---

## Setup Instructions

### Backend Setup

#### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 13+ running locally

#### 1. Configure the database connection

Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=project_db;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-key-that-is-at-least-32-characters-long-for-hs256",
    "Issuer": "WebAssignmentApp",
    "Audience": "WebAssignmentUsers",
    "ExpirationMinutes": 60
  }
}
```

#### 2. Restore dependencies
```bash
dotnet restore
```

#### 3. Apply database migrations
```bash
dotnet ef database update
```

#### 4. Run the backend
```bash
dotnet run
```

The API will be available at `http://localhost:5186`.

A default admin account is seeded automatically:
- **Username:** `admin`
- **Password:** `admin123`

---

### Frontend Setup

#### Prerequisites
- [Node.js 18+](https://nodejs.org/)

#### 1. Navigate to the client directory
```bash
cd client
```

#### 2. Install dependencies
```bash
npm install
```

#### 3. Start the development server
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`.

> The Vite dev server is configured to proxy all `/api` requests to `http://localhost:5186`, so the backend must be running first.

---

## Role-Based Access

| Feature | Admin | Instructor | Student |
|---|---|---|---|
| Browse courses | ✅ | ✅ | ✅ |
| Create / delete courses | ✅ | ❌ | ❌ |
| Edit course details | ✅ | ✅ (title & hours only) | ❌ |
| Manage students | ✅ (full edit) | ✅ (view only) | ❌ |
| Manage instructors | ✅ (full edit) | ✅ (view only) | ❌ |
| Enroll students in courses | ✅ | ❌ | ❌ |
| View course enrollment roster | ✅ | ✅ | ❌ |
| Assign grades | ✅ | ✅ | ❌ |
| View own enrolled courses | ❌ | ❌ | ✅ |
| Withdraw from a course | ❌ | ❌ | ✅ |

---

## Frontend Pages & Routes

| Route | Page | Access |
|---|---|---|
| `/` | Home — role-specific dashboard | Public |
| `/login` | Login form | Public |
| `/register` | Register form | Public |
| `/courses` | List of all courses | Public |
| `/courses/new` | Create a new course | Admin |
| `/courses/:id` | Edit course details | Admin, Instructor |
| `/students` | List of all students | Admin, Instructor |
| `/students/new` | Create a new student record | Admin |
| `/students/:id` | View / edit a student | Admin, Instructor |
| `/instructors` | List of all instructors | Admin, Instructor |
| `/instructors/new` | Create a new instructor | Admin |
| `/instructors/:id` | View / edit an instructor | Admin, Instructor |
| `/enrollments` | Role-aware enrollment view | All authenticated |
| `/enrollments/new` | Enroll a student in a course | Admin |

### Enrollment page by role
- **Admin** — select any student to view their courses and update grades
- **Instructor** — select any course to see enrolled students and assign grades
- **Student** — view their own enrolled courses and withdraw if needed

---

## API Endpoints

All endpoints are prefixed with `/api`. Authenticated endpoints require the header:
```
Authorization: Bearer <jwt_token>
```

### Authentication
| Method | Route | Access | Description |
|---|---|---|---|
| POST | `/auth/register` | Public | Register a new user (Student / Instructor / Admin) |
| POST | `/auth/login` | Public | Login and receive a JWT token |

### Courses
| Method | Route | Access | Description |
|---|---|---|---|
| GET | `/course` | Public | List all courses |
| GET | `/course/{id}` | Public | Get a single course |
| POST | `/course` | Admin | Create a course |
| PUT | `/course/{id}` | Admin, Instructor | Update a course |
| DELETE | `/course/{id}` | Admin | Delete a course |
| PUT | `/course/{courseId}/instructor/{instructorId}` | Admin | Assign instructor to a course |

### Students
| Method | Route | Access | Description |
|---|---|---|---|
| GET | `/student` | Admin, Instructor | List all students |
| GET | `/student/{id}` | Admin, Instructor, Student | Get a student by ID |
| POST | `/student` | Admin | Create a student record |
| PUT | `/student/{id}` | Admin, Student | Update a student record |
| DELETE | `/student/{studentId}/withdraw/{courseId}` | Student | Withdraw from a course |

### Instructors
| Method | Route | Access | Description |
|---|---|---|---|
| GET | `/instructor` | Authenticated | List all instructors |
| GET | `/instructor/{id}` | Authenticated | Get an instructor by ID |
| POST | `/instructor` | Admin | Create an instructor record |
| PUT | `/instructor/{id}` | Admin, Instructor | Update an instructor record |
| GET | `/instructor/{id}/courses` | Authenticated | Get courses taught by an instructor |
| GET | `/instructor/{id}/profile` | Authenticated | Get instructor profile |
| PUT | `/instructor/{id}/profile` | Admin, Instructor | Update instructor profile |

### Enrollments
| Method | Route | Access | Description |
|---|---|---|---|
| POST | `/enrollments` | Admin | Enroll a student in a course |
| GET | `/enrollments/student/{studentId}` | Authenticated | Get a student's enrolled courses |
| GET | `/enrollments/course/{courseId}` | Admin, Instructor | Get a course's enrolled students |
| PUT | `/enrollments/{studentId}/courses/{courseId}` | Admin, Instructor | Update a student's grade |

---

## Project Structure

```
WebAssignment/                   ← Backend (.NET)
├── Controllers/
│   ├── AuthController.cs
│   ├── CourseController.cs
│   ├── StudentController.cs
│   ├── EnrollmentController.cs
│   └── InstructorController.cs
├── Models/
│   ├── User.cs
│   ├── Student.cs
│   ├── Course.cs
│   ├── Enrollment.cs
│   ├── Instructor.cs
│   └── InstructorProfile.cs
├── DTOs/
├── Services/
├── Interfaces/
├── Database/
│   └── ApplicationDbContext.cs
├── Migrations/
├── Program.cs
└── appsettings.json

client/                          ← Frontend (React + Vite)
├── src/
│   ├── components/
│   │   ├── Navigation.jsx       ← Role-aware navbar
│   │   └── ProtectedRoute.jsx   ← Auth + role guard
│   ├── context/
│   │   └── AuthContext.jsx      ← Global auth state, login/logout
│   ├── pages/
│   │   ├── HomePage.jsx
│   │   ├── LoginPage.jsx
│   │   ├── RegisterPage.jsx
│   │   ├── CoursesPage.jsx
│   │   ├── CourseCreate.jsx
│   │   ├── CourseEdit.jsx
│   │   ├── StudentsPage.jsx
│   │   ├── StudentCreate.jsx
│   │   ├── StudentDetail.jsx
│   │   ├── InstructorsPage.jsx
│   │   ├── InstructorCreate.jsx
│   │   ├── InstructorDetail.jsx
│   │   ├── EnrollmentsPage.jsx
│   │   └── EnrollmentCreate.jsx
│   ├── services/
│   │   ├── api.js               ← Axios instance with JWT interceptor
│   │   ├── authService.js
│   │   ├── courseService.js
│   │   ├── studentService.js
│   │   ├── instructorService.js
│   │   └── enrollmentService.js
│   ├── utils/
│   │   └── jwt.js               ← JWT decode helper
│   ├── App.jsx                  ← Route definitions
│   └── main.jsx
├── package.json
└── vite.config.js               ← Proxy config for /api → localhost:5186
```

---

## Authentication & Security

The system uses **JWT Bearer tokens** for stateless authentication. On login or registration the server returns a token containing the user's role, and optionally their linked `StudentId` or `InstructorId`. The frontend stores this token in `localStorage` and attaches it to every request via an Axios interceptor.

### Token claims
| Claim | Value |
|---|---|
| `NameIdentifier` | User ID |
| `Name` | Username |
| `Email` | Email address |
| `Role` | `Admin`, `Instructor`, or `Student` |
| `StudentId` | Linked student record ID (if set) |
| `InstructorId` | Linked instructor record ID (if set) |

### Security note

Tokens are stored in `localStorage`, which is sufficient for a development/academic environment. For production, **HTTP-only cookies** are the recommended approach as they are inaccessible to JavaScript and protect against XSS attacks.
