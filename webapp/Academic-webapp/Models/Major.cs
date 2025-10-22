using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Majors", Schema = "Academic")]
public class Major
{
    [Key]
    public int MajorID { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Nombre de la carrera")]
    public string MajorName { get; set; } = string.Empty;

    [Display(Name = "Departamento")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un departamento.")]
    public int DepartmentID { get; set; }

    public Department? Department { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
