using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Enrollments", Schema = "Academic")]
public class Enrollment
{
    [Key]
    public int EnrollmentID { get; set; }

    [Display(Name = "Student")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a student.")]
    public int StudentID { get; set; }

    [Display(Name = "Course")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a course.")]
    public int CourseID { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public Course? Course { get; set; }
}
