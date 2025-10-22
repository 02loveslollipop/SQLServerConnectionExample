using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Departments;

public class DetailsModel : PageModel
{
    private readonly AcademicContext _context;

    public DetailsModel(AcademicContext context)
    {
        _context = context;
    }

    public Department Department { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var department = await _context.Departments
            .AsNoTracking()
            .Include(d => d.Majors)
            .Include(d => d.Instructors)
            .FirstOrDefaultAsync(d => d.DepartmentID == id);

        if (department is null)
        {
            return NotFound();
        }

        Department = department;
        return Page();
    }
}
