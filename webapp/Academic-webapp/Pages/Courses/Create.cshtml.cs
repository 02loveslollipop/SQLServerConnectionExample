using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Courses;

public class CreateModel : PageModel
{
    private readonly AcademicContext _context;

    public CreateModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Course Course { get; set; } = new();

    public SelectList InstructorOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateInstructorsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateInstructorsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Courses.Add(Course);
        await _context.SaveChangesAsync();

        StatusMessage = $"Course '{Course.CourseName}' created successfully.";
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
