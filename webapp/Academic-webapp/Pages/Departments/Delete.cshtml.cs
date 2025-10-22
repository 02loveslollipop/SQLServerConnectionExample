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

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = null)
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
        if (saveChangesError == true)
        {
            ErrorMessage = "No se puede eliminar el departamento porque esta referenciado por otros registros.";
        }
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

        try
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            StatusMessage = $"Departamento '{department.DepartmentName}' eliminado correctamente.";
            return RedirectToPage("Index");
        }
        catch (DbUpdateException)
        {
            return RedirectToPage(new { id, saveChangesError = true });
        }
    }
}
