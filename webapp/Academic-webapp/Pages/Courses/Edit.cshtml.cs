using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Courses;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Course Course { get; set; } = new();

    public SelectList InstructorOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CourseID == id);

        if (course is null)
        {
            return NotFound();
        }

        Course = course;
        await PopulateInstructorsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateInstructorsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Course).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Course '{Course.CourseName}' updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Courses.AnyAsync(c => c.CourseID == Course.CourseID))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateInstructorsAsync()
    {
        InstructorOptions = new SelectList(await _context.Instructors
            .OrderBy(i => i.InstructorName)
            .Select(i => new { i.InstructorID, Display = i.InstructorName + " (" + i.Department!.DepartmentName + ")" })
            .ToListAsync(), "InstructorID", "Display");
    }
}
