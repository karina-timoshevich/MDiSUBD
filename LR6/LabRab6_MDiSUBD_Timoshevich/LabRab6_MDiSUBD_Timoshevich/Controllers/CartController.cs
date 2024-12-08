using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;

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
    public async Task<IActionResult> Checkout()
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to proceed with checkout.";
            return RedirectToAction("Login", "Account");
        }

        /*var success = await _dbService.PlaceOrder(clientId.Value);
        if (success)
        {
            TempData["SuccessMessage"] = "Your order has been placed.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to place your order. Please try again.";
        }*/
        return RedirectToAction("Index");
    }
}
