using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Instructors;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Instructor Instructor { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = null)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .Include(i => i.Department)
            .FirstOrDefaultAsync(i => i.InstructorID == id);

        if (instructor is null)
        {
            return NotFound();
        }

        Instructor = instructor;
        if (saveChangesError == true)
        {
            ErrorMessage = "Unable to delete instructor because it is referenced by other records.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorID == id);
        if (instructor is null)
        {
            return NotFound();
        }

        try
        {
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            StatusMessage = $"Instructor '{instructor.InstructorName}' deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (DbUpdateException)
        {
            return RedirectToPage(new { id, saveChangesError = true });
        }
    }
}
