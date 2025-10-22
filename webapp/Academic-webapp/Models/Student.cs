using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Students", Schema = "Academic")]
public class Student
{
    [Key]
    public int StudentID { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Major")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a major.")]
    public int MajorID { get; set; }

    public Major? Major { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
