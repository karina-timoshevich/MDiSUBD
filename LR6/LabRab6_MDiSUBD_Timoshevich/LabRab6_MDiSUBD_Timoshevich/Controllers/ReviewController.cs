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

    // Просмотр отзывов
    public async Task<IActionResult> Index()
    {
        var reviews = await _dbService.GetReviews();
        return View(reviews);
    }

    // Добавление отзыва
    [HttpPost]
    public async Task<IActionResult> AddReview(int clientId, int rating, string text)
    {
        var success = await _dbService.AddReview(clientId, rating, text);
        if (success)
            return RedirectToAction("Index");

        return View("Error");
    }
}