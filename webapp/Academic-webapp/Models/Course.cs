using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Courses", Schema = "Academic")]
public class Course
{
    [Key]
    public int CourseID { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Credits { get; set; }

    [Display(Name = "Instructor")]
    [Range(1, int.MaxValue, ErrorMessage = "Select an instructor.")]
    public int InstructorID { get; set; }

    public Instructor? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
