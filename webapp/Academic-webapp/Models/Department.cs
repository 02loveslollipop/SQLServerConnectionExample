using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Departments", Schema = "Academic")]
public class Department
{
    [Key]
    public int DepartmentID { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    public ICollection<Major> Majors { get; set; } = new List<Major>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
