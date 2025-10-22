"""Script para generar y cargar datos sintéticos en SQL Server."""

import os
import random
from datetime import datetime, timedelta

import pandas as pd
import pyodbc


def build_connection_string() -> str:
    """Construye el connection string utilizando variables de entorno o valores por defecto."""
    host = os.getenv("ACADEMIC_DB_HOST", "aaa.bbbb.cccc.dddd")
    port = os.getenv("ACADEMIC_DB_PORT", "1433")
    database = os.getenv("ACADEMIC_DB_NAME", "AcademicDB")
    user = os.getenv("ACADEMIC_DB_USER", "admin")
    password = os.getenv("ACADEMIC_DB_PASSWORD", "password")
    driver = os.getenv("ACADEMIC_DB_DRIVER", "{ODBC Driver 18 for SQL Server}")

    server = f"{host},{port}"
    return (
        f"DRIVER={driver};SERVER={server};DATABASE={database};"
        f"UID={user};PWD={password};Encrypt=yes;TrustServerCertificate=yes"
    )


def generate_and_load_data(conn_str: str) -> None:
    """Genera datos sintéticos y los inserta en la base de datos."""
    random.seed(2025)

    department_names = [
        "Ingeniería y Tecnología",
        "Ciencias de la Salud",
        "Ciencias Sociales",
        "Artes y Humanidades",
        "Ciencias Empresariales",
    ]

    majors_by_department = {
        "Ingeniería y Tecnología": [
            "Ingeniería de Software",
            "Ingeniería de Sistemas",
            "Ciencia de Datos",
        ],
        "Ciencias de la Salud": [
            "Medicina",
            "Enfermería",
            "Biotecnología",
        ],
        "Ciencias Sociales": [
            "Psicología",
            "Sociología",
            "Relaciones Internacionales",
        ],
        "Artes y Humanidades": [
            "Historia del Arte",
            "Literatura",
            "Filosofía",
        ],
        "Ciencias Empresariales": [
            "Administración de Empresas",
            "Economía",
            "Finanzas",
        ],
    }

    first_names = [
        "Alejandro",
        "María",
        "Carlos",
        "Lucía",
        "Javier",
        "Sofía",
        "Andrés",
        "Camila",
        "Diego",
        "Valentina",
        "Luis",
        "Paula",
        "Héctor",
        "Daniela",
        "Raúl",
        "Elena",
        "Felipe",
        "Isabel",
    ]
    last_names = [
        "García",
        "Martínez",
        "Rodríguez",
        "López",
        "González",
        "Pérez",
        "Sánchez",
        "Ramírez",
        "Flores",
        "Torres",
        "Vargas",
        "Silva",
        "Castro",
        "Romero",
        "Navarro",
        "Maldonado",
        "Muñoz",
        "Ortiz",
    ]

    course_prefixes = [
        "Introducción a",
        "Fundamentos de",
        "Seminario en",
        "Taller de",
        "Laboratorio de",
        "Teoría de",
        "Aplicaciones de",
        "Métodos en",
    ]
    course_topics = [
        "Algoritmos",
        "Salud Pública",
        "Economía Aplicada",
        "Creatividad Digital",
        "Gestión Empresarial",
        "Neurociencia",
        "Analítica de Datos",
        "Derecho Comparado",
        "Historia Moderna",
        "Finanzas Cuantitativas",
        "Educación Inclusiva",
        "Comunicación Estratégica",
    ]

    enrollments_range = (3, 5)

    with pyodbc.connect(conn_str, timeout=15) as conn:
        conn.autocommit = False
        cursor = conn.cursor()
        cursor.fast_executemany = True
        try:
            print("Limpiando datos previos...")
            cursor.execute("DELETE FROM Academic.Enrollments")
            cursor.execute("DELETE FROM Academic.Students")
            cursor.execute("DELETE FROM Academic.Courses")
            cursor.execute("DELETE FROM Academic.Majors")
            cursor.execute("DELETE FROM Academic.Instructors")
            cursor.execute("DELETE FROM Academic.Departments")

            for table in [
                "Academic.Enrollments",
                "Academic.Students",
                "Academic.Courses",
                "Academic.Majors",
                "Academic.Instructors",
                "Academic.Departments",
            ]:
                cursor.execute(f"DBCC CHECKIDENT ('{table}', RESEED, 0)")

            print("Insertando departamentos...")
            cursor.executemany(
                "INSERT INTO Academic.Departments (DepartmentName) VALUES (?)",
                [(name,) for name in department_names],
            )
            cursor.execute("SELECT DepartmentID, DepartmentName FROM Academic.Departments")
            department_lookup = {
                row.DepartmentName: row.DepartmentID for row in cursor.fetchall()
            }

            print("Insertando carreras (majors)...")
            major_rows = []
            for dept_name, majors in majors_by_department.items():
                for major_name in majors:
                    major_rows.append((major_name, department_lookup[dept_name]))
            cursor.executemany(
                "INSERT INTO Academic.Majors (MajorName, DepartmentID) VALUES (?, ?)",
                major_rows,
            )
            cursor.execute("SELECT MajorID FROM Academic.Majors")
            major_ids = [row.MajorID for row in cursor.fetchall()]
            if not major_ids:
                raise RuntimeError("No se pudieron crear las carreras.")

            print("Insertando docentes...")
            instructor_rows = []
            dept_ids = list(department_lookup.values())
            for _ in range(100):
                full_name = f"{random.choice(first_names)} {random.choice(last_names)}"
                dept_id = random.choice(dept_ids)
                instructor_rows.append((full_name, dept_id))
            cursor.executemany(
                "INSERT INTO Academic.Instructors (InstructorName, DepartmentID) VALUES (?, ?)",
                instructor_rows,
            )
            cursor.execute("SELECT InstructorID FROM Academic.Instructors")
            instructor_ids = [row.InstructorID for row in cursor.fetchall()]
            if not instructor_ids:
                raise RuntimeError("No se pudieron crear los docentes.")

            print("Insertando cursos...")
            course_rows = []
            course_counter = 0
            for instructor_id in instructor_ids:
                for _ in range(random.randint(1, 3)):
                    course_counter += 1
                    course_name = (
                        f"{random.choice(course_prefixes)} "
                        f"{random.choice(course_topics)} {course_counter:03d}"
                    )
                    credits = random.randint(2, 5)
                    course_rows.append((course_name, credits, instructor_id))
            cursor.executemany(
                "INSERT INTO Academic.Courses (CourseName, Credits, InstructorID) VALUES (?, ?, ?)",
                course_rows,
            )
            cursor.execute("SELECT CourseID FROM Academic.Courses")
            course_ids = [row.CourseID for row in cursor.fetchall()]
            if not course_ids:
                raise RuntimeError("No se pudieron crear los cursos.")

            print("Insertando estudiantes...")
            student_rows = []
            for _ in range(5000):
                first = random.choice(first_names)
                last = random.choice(last_names)
                major_id = random.choice(major_ids)
                student_rows.append((first, last, major_id))
            cursor.executemany(
                "INSERT INTO Academic.Students (FirstName, LastName, MajorID) VALUES (?, ?, ?)",
                student_rows,
            )
            cursor.execute("SELECT StudentID FROM Academic.Students")
            student_ids = [row.StudentID for row in cursor.fetchall()]
            if not student_ids:
                raise RuntimeError("No se pudieron crear los estudiantes.")

            print("Insertando inscripciones...")
            min_courses, max_courses = enrollments_range
            total_courses = len(course_ids)
            min_courses = min(min_courses, total_courses)
            max_courses = min(max_courses, total_courses)
            if max_courses == 0:
                raise RuntimeError(
                    "No hay cursos suficientes para generar inscripciones."
                )

            enrollment_rows = []
            today = datetime.utcnow()
            for student_id in student_ids:
                num_courses = random.randint(min_courses, max_courses)
                courses_for_student = random.sample(course_ids, k=num_courses)
                base_date = today - timedelta(days=random.randint(0, 365))
                for course_id in courses_for_student:
                    enrollment_date = base_date - timedelta(days=random.randint(0, 30))
                    enrollment_rows.append((student_id, course_id, enrollment_date))
            cursor.executemany(
                "INSERT INTO Academic.Enrollments (StudentID, CourseID, EnrollmentDate) VALUES (?, ?, ?)",
                enrollment_rows,
            )

            conn.commit()
            print("Datos sintéticos generados y cargados correctamente.")
            print(f"Total de inscripciones creadas: {len(enrollment_rows):,}")
        except Exception as exc:
            conn.rollback()
            print("Ocurrió un error y se revierte la transacción:", exc)
            raise


def show_summary(conn_str: str) -> None:
    """Muestra un resumen de totales y una muestra de estudiantes."""
    summary_queries = {
        "Departamentos": "SELECT COUNT(*) AS total FROM Academic.Departments",
        "Carreras": "SELECT COUNT(*) AS total FROM Academic.Majors",
        "Docentes": "SELECT COUNT(*) AS total FROM Academic.Instructors",
        "Cursos": "SELECT COUNT(*) AS total FROM Academic.Courses",
        "Estudiantes": "SELECT COUNT(*) AS total FROM Academic.Students",
        "Inscripciones": "SELECT COUNT(*) AS total FROM Academic.Enrollments",
    }

    with pyodbc.connect(conn_str, timeout=10) as conn:
        summary = {
            name: pd.read_sql(query, conn).iloc[0]["total"]
            for name, query in summary_queries.items()
        }
        print(pd.DataFrame.from_dict(summary, orient="index", columns=["Total"]))

        sample = pd.read_sql(
            "SELECT TOP 5 StudentID, FirstName, LastName, MajorID "
            "FROM Academic.Students ORDER BY NEWID()",
            conn,
        )
        print("\nMuestra de estudiantes:")
        print(sample)


def main() -> None:
    conn_str = build_connection_string()
    generate_and_load_data(conn_str)
    show_summary(conn_str)


if __name__ == "__main__":
    main()