using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Student Student { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .AsNoTracking()
            .Include(s => s.Major)
                .ThenInclude(m => m!.Department)
            .FirstOrDefaultAsync(s => s.StudentID == id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id);
        if (student is null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        StatusMessage = $"Student '{student.FirstName} {student.LastName}' deleted successfully.";

        return RedirectToPage("Index");
    }
}
