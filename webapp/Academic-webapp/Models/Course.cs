using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Courses", Schema = "Academic")]
public class Course
{
    [Key]
    public int CourseID { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Nombre del curso")]
    public string CourseName { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "{0} debe estar entre {1} y {2}.")]
    public int Credits { get; set; }

    [Display(Name = "Docente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un docente.")]
    public int InstructorID { get; set; }

    public Instructor? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
