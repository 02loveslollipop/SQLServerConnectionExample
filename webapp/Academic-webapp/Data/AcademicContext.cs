using Academic_webapp.Models;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Data;

public class AcademicContext(DbContextOptions<AcademicContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments", "Academic", table =>
            {
                table.HasCheckConstraint("CK_DepartmentName_Length", "LEN([DepartmentName]) >= 2");
            });
            entity.HasIndex(d => d.DepartmentName).IsUnique();
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.ToTable("Majors", "Academic");
            entity.HasIndex(m => m.MajorName).IsUnique();
            entity.HasOne(m => m.Department)
                  .WithMany(d => d.Majors)
                  .HasForeignKey(m => m.DepartmentID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructors", "Academic", table =>
            {
                table.HasCheckConstraint("CK_InstructorName_Length", "LEN([InstructorName]) >= 2");
            });
            entity.HasOne(i => i.Department)
                  .WithMany(d => d.Instructors)
                  .HasForeignKey(i => i.DepartmentID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students", "Academic", table =>
            {
                table.HasCheckConstraint("CK_StudentName_Length", "LEN([FirstName]) >= 2 AND LEN([LastName]) >= 2");
            });
            entity.HasOne(s => s.Major)
                  .WithMany(m => m.Students)
                  .HasForeignKey(s => s.MajorID)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Property(s => s.FirstName).HasMaxLength(50);
            entity.Property(s => s.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses", "Academic", table =>
            {
                table.HasCheckConstraint("CK_Course_Credits", "[Credits] > 0 AND [Credits] <= 5");
            });
            entity.HasIndex(c => c.CourseName).IsUnique();
            entity.HasOne(c => c.Instructor)
                  .WithMany(i => i.Courses)
                  .HasForeignKey(c => c.InstructorID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.ToTable("Enrollments", "Academic", table =>
            {
                table.HasCheckConstraint("CK_EnrollDate_NotFuture", "[EnrollmentDate] <= SYSUTCDATETIME()");
            });
            entity.Property(e => e.EnrollmentDate)
                  .HasColumnType("datetime2");
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentID)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseID)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
