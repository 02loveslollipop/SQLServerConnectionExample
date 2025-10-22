using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Instructors", Schema = "Academic")]
public class Instructor
{
    [Key]
    public int InstructorID { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Nombre del docente")]
    public string InstructorName { get; set; } = string.Empty;

    [Display(Name = "Departamento")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un departamento.")]
    public int DepartmentID { get; set; }

    public Department? Department { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
