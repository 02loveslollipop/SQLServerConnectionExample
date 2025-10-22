using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Students;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Student Student { get; set; } = new();

    public SelectList MajorOptions { get; private set; } = default!;

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
            .FirstOrDefaultAsync(s => s.StudentID == id);

        if (student is null)
        {
            return NotFound();
        }

        Student = student;
        await PopulateMajorsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateMajorsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Student).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Estudiante '{Student.FullName}' actualizado correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Students.AnyAsync(s => s.StudentID == Student.StudentID))
            {
                return NotFound();
            }

            throw;
        }

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
