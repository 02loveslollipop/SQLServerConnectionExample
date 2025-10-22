using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academic_webapp.Models;

[Table("Enrollments", Schema = "Academic")]
public class Enrollment
{
    [Key]
    public int EnrollmentID { get; set; }

    [Display(Name = "Estudiante")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estudiante.")]
    public int StudentID { get; set; }

    [Display(Name = "Curso")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un curso.")]
    public int CourseID { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inscripción")]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public Student? Student { get; set; }
    public Course? Course { get; set; }
}
