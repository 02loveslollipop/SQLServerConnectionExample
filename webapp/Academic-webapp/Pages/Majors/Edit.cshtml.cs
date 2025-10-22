using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Majors;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Major Major { get; set; } = new();

    public SelectList DepartmentOptions { get; private set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var major = await _context.Majors
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MajorID == id);

        if (major is null)
        {
            return NotFound();
        }

        Major = major;
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

        _context.Attach(Major).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Major '{Major.MajorName}' updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Majors.AnyAsync(m => m.MajorID == Major.MajorID))
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
