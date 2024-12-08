using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CartController : Controller
{
    private readonly DbService _dbService;

    public CartController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to view your cart.";
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _dbService.GetCartItemsByClientId(clientId.Value);
        var pickupLocations = await _dbService.GetAllPickupLocations();
        var promoCodes = await _dbService.GetAllPromoCodes();
        ViewBag.PickupLocations = new SelectList(pickupLocations, "Id", "Name");
        ViewBag.PromoCodes = new SelectList(promoCodes, "Id", "Code"); 

        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(int productId, int quantity)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to update your cart.";
            return RedirectToAction("Login", "Account");
        }

        await _dbService.UpdateCart(clientId.Value, productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to remove items from your cart.";
            return RedirectToAction("Login", "Account");
        }

        await _dbService.RemoveFromCart( productId, clientId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int pickupLocationId, int? promoCodeId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to place an order.";
            return RedirectToAction("Login", "Account");
        }

        await _dbService.CreateOrder(clientId.Value, pickupLocationId, promoCodeId);

        TempData["SuccessMessage"] = "Your order has been placed successfully.";
        return RedirectToAction("Index", "Cart");
    }
}
