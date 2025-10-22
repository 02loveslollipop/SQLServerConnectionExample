using Academic_webapp.Data;
using Academic_webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Academic_webapp.Pages.Enrollments;

public class IndexModel : PageModel
{
    private readonly AcademicContext _context;

    public IndexModel(AcademicContext context)
    {
        _context = context;
    }

    public IList<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    private const int PageSize = 100;

    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public int DisplayFrom { get; private set; }
    public int DisplayTo { get; private set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(int pageNumber = 1)
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c!.Instructor)
            .OrderByDescending(e => e.EnrollmentDate);

        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        if (TotalPages == 0)
        {
            TotalPages = 1;
        }

        CurrentPage = pageNumber < 1 ? 1 : pageNumber;
        if (CurrentPage > TotalPages)
        {
            CurrentPage = TotalPages;
        }
        var skip = (CurrentPage - 1) * PageSize;

        Enrollments = await query
            .Skip(skip)
            .Take(PageSize)
            .ToListAsync();

        if (TotalCount == 0)
        {
            DisplayFrom = 0;
            DisplayTo = 0;
        }
        else
        {
            DisplayFrom = skip + 1;
            DisplayTo = skip + Enrollments.Count;
        }
    }
}
