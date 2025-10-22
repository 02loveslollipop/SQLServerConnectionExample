using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Majors;

public class DetailsModel : PageModel
{
    private readonly AcademicContext _context;

    public DetailsModel(AcademicContext context)
    {
        _context = context;
    }

    public Major Major { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var major = await _context.Majors
            .AsNoTracking()
            .Include(m => m.Department)
            .Include(m => m.Students)
            .FirstOrDefaultAsync(m => m.MajorID == id);

        if (major is null)
        {
            return NotFound();
        }

        Major = major;
        return Page();
    }
}
