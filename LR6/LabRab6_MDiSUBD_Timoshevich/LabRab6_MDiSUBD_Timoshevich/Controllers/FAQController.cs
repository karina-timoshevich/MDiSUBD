using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;

namespace LabRab6_MDiSUBD_Timoshevich.Controllers;

public class FAQController : Controller
{
    private readonly DbService _dbService;

    public FAQController(DbService dbService)
    {
        _dbService = dbService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string question, string answer)
    {
        if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
        {
            ViewBag.Error = "Both Question and Answer fields are required.";
            return View();
        }

        var success = await _dbService.AddFAQAsync(question, answer);

        if (success)
        {
            ViewBag.Message = "FAQ entry added successfully.";
        }
        else
        {
            ViewBag.Error = "An error occurred while adding the FAQ entry.";
        }

        return View();
    }
}