using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Students;

public class CreateModel : PageModel
{
    private readonly AcademicContext _context;

    public CreateModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Student Student { get; set; } = new();

    public SelectList MajorOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateMajorsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateMajorsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Students.Add(Student);
        await _context.SaveChangesAsync();

    StatusMessage = $"Estudiante '{Student.FullName}' registrado correctamente.";
        return RedirectToPage("Index");
    }

    private async Task PopulateMajorsAsync()
    {
        MajorOptions = new SelectList(await _context.Majors
            .OrderBy(m => m.MajorName)
            .Select(m => new { m.MajorID, Display = m.MajorName + " (" + m.Department!.DepartmentName + ")" })
            .ToListAsync(), "MajorID", "Display");
    }
}
