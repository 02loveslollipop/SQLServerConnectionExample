using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Departments;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Department> Departments { get; private set; } = new List<Department>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Departments = await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }
}
