using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Instructors;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Instructor Instructor { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.InstructorID == id);

        if (instructor is null)
        {
            return NotFound();
        }

        Instructor = instructor;
        await PopulateDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Instructor).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Docente '{Instructor.InstructorName}' actualizado correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Instructors.AnyAsync(i => i.InstructorID == Instructor.InstructorID))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateDepartmentsAsync()
    {
        DepartmentOptions = new SelectList(await _context.Departments
            .OrderBy(d => d.DepartmentName)
            .Select(d => new { d.DepartmentID, d.DepartmentName })
            .ToListAsync(), "DepartmentID", "DepartmentName");
    }
}
