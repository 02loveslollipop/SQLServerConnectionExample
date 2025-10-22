using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Students", Schema = "Academic")]
public class Student
{
    [Key]
    public int StudentID { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Carrera")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una carrera.")]
    public int MajorID { get; set; }

    public Major? Major { get; set; }

    [Display(Name = "Nombre completo")]
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
