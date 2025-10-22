using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Departments;

public class EditModel : PageModel
{
    private readonly AcademicContext _context;

    public EditModel(AcademicContext context)
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Department).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Departamento '{Department.DepartmentName}' actualizado correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Departments.AnyAsync(d => d.DepartmentID == Department.DepartmentID))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToPage("Index");
    }
}
