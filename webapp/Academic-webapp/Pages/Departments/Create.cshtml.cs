using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Academic_webapp.Pages.Departments;

public class CreateModel : PageModel
{
    private readonly AcademicContext _context;

    public CreateModel(AcademicContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Department Department { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Departments.Add(Department);
        await _context.SaveChangesAsync();

    StatusMessage = $"Departamento '{Department.DepartmentName}' creado correctamente.";
        return RedirectToPage("Index");
    }
}
