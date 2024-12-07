using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;

public class PickupLocationController : Controller
{
    private readonly DbService _dbService;

    public PickupLocationController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var pickupLocations = await _dbService.GetAllPickupLocations();
        return View(pickupLocations);
    }
}
