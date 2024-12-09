using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace LabRab6_MDiSUBD_Timoshevich.Controllers;

public class JobController : Controller
{
    private readonly DbService _dbService;

    public JobController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var jobs = await _dbService.GetAllJobsAsync();
        return View(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> AddJob(string title, string description)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
        {
            await _dbService.AddJobAsync(title, description);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EditJob(int id, string title, string description)
    {
        if (id > 0 && !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
        {
            await _dbService.UpdateJobAsync(id, title, description);
        }

        return RedirectToAction("Index");
    }
}
