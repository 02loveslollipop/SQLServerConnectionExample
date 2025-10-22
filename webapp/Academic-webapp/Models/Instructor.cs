using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Instructors", Schema = "Academic")]
public class Instructor
{
    [Key]
    public int InstructorID { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Instructor Name")]
    public string InstructorName { get; set; } = string.Empty;

    [Display(Name = "Department")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a department.")]
    public int DepartmentID { get; set; }

    public Department? Department { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
