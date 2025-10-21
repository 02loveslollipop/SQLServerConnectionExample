CREATE DATABASE AcademicDB;
GO
USE AcademicDB;
GO

-- Create schema
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'Academic')
    EXEC('CREATE SCHEMA Academic');
GO

/* ===============================
    TABLES
   =============================== */

-- Departments
CREATE TABLE Academic.Departments (
    DepartmentID   INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    CONSTRAINT CK_DepartmentName CHECK (LEN(DepartmentName) >= 2)
);

-- Majors (belong to a Department)
CREATE TABLE Academic.Majors (
    MajorID   INT IDENTITY(1,1) PRIMARY KEY,
    MajorName NVARCHAR(100) NOT NULL UNIQUE,
    DepartmentID INT NOT NULL,
    CONSTRAINT FK_Major_Department FOREIGN KEY (DepartmentID)
        REFERENCES Academic.Departments(DepartmentID)
);

-- Instructors (belong to a Department)
CREATE TABLE Academic.Instructors (
    InstructorID   INT IDENTITY(1,1) PRIMARY KEY,
    InstructorName NVARCHAR(100) NOT NULL,
    DepartmentID   INT NOT NULL,
    CONSTRAINT CK_InstructorName CHECK (LEN(InstructorName) >= 2),
    CONSTRAINT FK_Instructor_Department FOREIGN KEY (DepartmentID)
        REFERENCES Academic.Departments(DepartmentID)
);

-- Students (now reference MajorID)
CREATE TABLE Academic.Students (
    StudentID  INT IDENTITY(1,1) PRIMARY KEY,
    FirstName  NVARCHAR(50) NOT NULL,
    LastName   NVARCHAR(50) NOT NULL,
    MajorID    INT NOT NULL,
    CONSTRAINT CK_StudentName CHECK (LEN(FirstName) >= 2 AND LEN(LastName) >= 2),
    CONSTRAINT FK_Student_Major FOREIGN KEY (MajorID)
        REFERENCES Academic.Majors(MajorID)
);

-- Courses (belong to Instructors)
CREATE TABLE Academic.Courses (
    CourseID     INT IDENTITY(1,1) PRIMARY KEY,
    CourseName   NVARCHAR(100) NOT NULL UNIQUE,
    Credits      INT NOT NULL,
    InstructorID INT NOT NULL,
    CONSTRAINT CK_Credits CHECK (Credits > 0 AND Credits <= 5),
    CONSTRAINT FK_Course_Instructor FOREIGN KEY (InstructorID)
        REFERENCES Academic.Instructors(InstructorID)
);

-- Enrollments
CREATE TABLE Academic.Enrollments (
    EnrollmentID   INT IDENTITY(1,1) PRIMARY KEY,
    StudentID      INT NOT NULL,
    CourseID       INT NOT NULL,
    EnrollmentDate DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Enroll_Student FOREIGN KEY (StudentID) REFERENCES Academic.Students(StudentID),
    CONSTRAINT FK_Enroll_Course  FOREIGN KEY (CourseID)  REFERENCES Academic.Courses(CourseID),
    CONSTRAINT CK_EnrollDate CHECK (EnrollmentDate <= SYSUTCDATETIME())
);
GO

/* ===============================
   SAMPLE DATA
   =============================== */

-- Departments
INSERT INTO Academic.Departments (DepartmentName) VALUES
(N'Computer Science'),
(N'Mathematics'),
(N'Physics'),
(N'Literature');

-- Majors (linked to departments)
INSERT INTO Academic.Majors (MajorName, DepartmentID) VALUES
(N'Computer Science', 1),
(N'Software Engineering', 1),
(N'Mathematics', 2),
(N'Applied Mathematics', 2),
(N'Physics', 3),
(N'Literature', 4);

-- Instructors
INSERT INTO Academic.Instructors (InstructorName, DepartmentID) VALUES
(N'Dr. Emily Davis', 1),
(N'Prof. Jonathan Reed', 1),
(N'Dr. Michael Wilson', 2),
(N'Dr. Ana Martínez', 2),
(N'Dr. Sarah Lee', 3),
(N'Prof. Laura Gómez', 4);

-- Students (reference Majors)
INSERT INTO Academic.Students (FirstName, LastName, MajorID) VALUES
(N'Alice',   N'Johnson', 1), -- CS
(N'Bob',     N'Smith',   3), -- Math
(N'Charlie', N'Brown',   5), -- Physics
(N'Diana',   N'Nguyen',  6), -- Literature
(N'Erik',    N'Garcia',  2), -- Software Eng
(N'Fiona',   N'Lopez',   4), -- Applied Math
(N'Hiro',    N'Sato',    5), -- Physics
(N'Inés',    N'Ramírez', 1); -- CS

-- Courses (InstructorID references above)
INSERT INTO Academic.Courses (CourseName, Credits, InstructorID) VALUES
(N'Introduction to Programming', 4, 1),
(N'Data Structures',             4, 2),
(N'Calculus I',                  3, 3),
(N'Linear Algebra',              3, 4),
(N'General Physics',             4, 5),
(N'World Literature',            3, 6);

-- Enrollments
INSERT INTO Academic.Enrollments (StudentID, CourseID, EnrollmentDate) VALUES
(1, 1, '2024-08-15'), -- Alice
(1, 2, '2024-08-16'),
(2, 3, '2024-08-16'),
(2, 4, '2024-08-17'),
(3, 5, '2024-08-15'),
(4, 6, '2024-08-18'),
(5, 1, '2024-08-20'),
(5, 2, '2024-08-21'),
(6, 3, '2024-08-22'),
(7, 5, '2024-08-22'),
(8, 1, '2024-08-23'),
(8, 4, '2024-08-23');
GO

-- Example queries

-- 1. Enrolled students per deparment

SELECT
    d.DepartmentName,
    COUNT(DISTINCT e.StudentID) AS StudentsEnrolled
FROM Academic.Enrollments e
JOIN Academic.Students s     ON s.StudentID = e.StudentID
JOIN Academic.Majors m       ON m.MajorID = s.MajorID
JOIN Academic.Departments d  ON d.DepartmentID = m.DepartmentID
GROUP BY d.DepartmentName
ORDER BY StudentsEnrolled DESC;

-- 2. Enrolled students per course and instructor

SELECT
    d.DepartmentName,
    c.CourseName,
    i.InstructorName,
    COUNT(e.StudentID) AS TotalEnrollments
FROM Academic.Enrollments e
JOIN Academic.Courses     c ON e.CourseID = c.CourseID
JOIN Academic.Instructors i ON c.InstructorID = i.InstructorID
JOIN Academic.Departments d ON i.DepartmentID = d.DepartmentID
GROUP BY d.DepartmentName, c.CourseName, i.InstructorName
ORDER BY d.DepartmentName, c.CourseName;

-- 3. Students and their major (Name formated)

SELECT
    s.FirstName + ' ' + s.LastName AS Student,
    m.MajorName,
    d.DepartmentName
FROM Academic.Students s
JOIN Academic.Majors m ON s.MajorID = m.MajorID
JOIN Academic.Departments d ON m.DepartmentID = d.DepartmentID
ORDER BY d.DepartmentName, Student;


