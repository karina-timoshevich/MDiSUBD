using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;

public class CatalogController : Controller
{
    private readonly DbService _dbService;

    public CatalogController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Catalog()
    {
        var products = await _dbService.GetAllProducts();
        return View(products);
    }
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)  
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to add items to your cart.";
            return RedirectToAction("Login", "Account");
        }

        var product = await _dbService.GetProductById(productId);
        if (product != null)
        {
            await _dbService.AddProductToCart(clientId.Value, productId, quantity);
            TempData["SuccessMessage"] = "Product added to cart!";
        }
        else
        {
            TempData["ErrorMessage"] = "Product not found.";
        }

        return RedirectToAction("Catalog");
    }

}