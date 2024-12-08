using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;

public class ReviewController : Controller
{
    private readonly DbService _dbService;

    public ReviewController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var reviews = await _dbService.GetReviews();
        return View(reviews);
    }

    [HttpGet]
    public IActionResult AddReview()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(int rating, string text)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to add a review.";
            return RedirectToAction("Login", "Account");
        }

        // Передаем три параметра в метод AddReview сервиса
        var isSuccess = await _dbService.AddReview(clientId.Value, rating, text);

        if (isSuccess)
        {
            TempData["SuccessMessage"] = "Your review has been added.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to add your review. Please try again.";
        }

        return RedirectToAction("Index"); // Возврат к списку отзывов
    }

}