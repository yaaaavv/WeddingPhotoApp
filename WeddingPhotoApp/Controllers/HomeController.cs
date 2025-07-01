using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController(ApplicationDbContext context) : Controller
{
    public IActionResult Index() => View();

    public readonly ApplicationDbContext _context = context;

    public IActionResult Gallery(int page = 1)
    {
        const int PageSize = 30;
        var totalPhotos = _context.Photos.Count();
        var totalPages = (int)Math.Ceiling((double)totalPhotos / PageSize);

        var photos = _context.Photos
            .OrderBy(p => p.PhotoId)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(photos);
    }


    public IActionResult Comments()
    {
        var photosWithComments = _context.Photos
            .Include(p => p.Comments)
            .Where(p => p.Comments.Any())
            .ToList();

        return View(photosWithComments);
    }


}