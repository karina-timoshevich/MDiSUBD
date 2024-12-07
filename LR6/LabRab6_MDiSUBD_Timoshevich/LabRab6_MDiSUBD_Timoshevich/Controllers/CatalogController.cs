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
}
