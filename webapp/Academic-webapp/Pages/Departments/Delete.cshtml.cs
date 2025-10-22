using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Departments;

public class DeleteModel : PageModel
{
    private readonly AcademicContext _context;

    public DeleteModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Department Department { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var department = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.DepartmentID == id);
        if (department is null)
        {
            return NotFound();
        }

        Department = department;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentID == id);
        if (department is null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        StatusMessage = $"Department '{department.DepartmentName}' deleted successfully.";

        return RedirectToPage("Index");
    }
}
