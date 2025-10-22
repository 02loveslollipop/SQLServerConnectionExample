using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Majors;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Major Major { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var major = await _context.Majors
            .AsNoTracking()
            .Include(m => m.Department)
            .FirstOrDefaultAsync(m => m.MajorID == id);

        if (major is null)
        {
            return NotFound();
        }

        Major = major;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var major = await _context.Majors.FirstOrDefaultAsync(m => m.MajorID == id);
        if (major is null)
        {
            return NotFound();
        }

        try
        {
            _context.Majors.Remove(major);
            await _context.SaveChangesAsync();
            StatusMessage = $"Major '{major.MajorName}' deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (DbUpdateException)
        {
            ErrorMessage = "Unable to delete major because it is referenced by other records.";
            Major = major;
            await _context.Entry(major).Reference(m => m.Department).LoadAsync();
            return Page();
        }
    }
}
