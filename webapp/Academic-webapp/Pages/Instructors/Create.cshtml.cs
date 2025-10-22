using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Instructors;

public class CreateModel : PageModel
{
    private readonly AcademicContext _context;

    public CreateModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Instructor Instructor { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateDepartmentsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await PopulateDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Instructors.Add(Instructor);
        await _context.SaveChangesAsync();

        StatusMessage = $"Instructor '{Instructor.InstructorName}' created successfully.";
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
