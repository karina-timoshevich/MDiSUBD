using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;

public class OrdersController : Controller
{
    private readonly DbService _dbService;

    public OrdersController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to view your orders.";
            return RedirectToAction("Login", "Account");
        }

        var orders = await _dbService.GetOrdersByClientId(clientId.Value);
        return View(orders);
    }
}