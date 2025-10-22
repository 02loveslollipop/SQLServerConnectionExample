using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Majors", Schema = "Academic")]
public class Major
{
    [Key]
    public int MajorID { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Major Name")]
    public string MajorName { get; set; } = string.Empty;

    [Display(Name = "Department")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a department.")]
    public int DepartmentID { get; set; }

    public Department? Department { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
