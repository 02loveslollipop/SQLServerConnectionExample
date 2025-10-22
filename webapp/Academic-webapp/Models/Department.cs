using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Departments", Schema = "Academic")]
public class Department
{
    [Key]
    public int DepartmentID { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Nombre del departamento")]
    public string DepartmentName { get; set; } = string.Empty;

    public ICollection<Major> Majors { get; set; } = new List<Major>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
