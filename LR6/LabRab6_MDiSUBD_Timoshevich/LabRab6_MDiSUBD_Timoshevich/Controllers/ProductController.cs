using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;

namespace LabRab6_MDiSUBD_Timoshevich.Controllers;

public class ProductController : Controller
{
    private readonly DbService _dbService;

    public ProductController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _dbService.GetAllProducts();
        return View(products);
    }
    private async Task PopulateDropdowns()
    {
        ViewData["ProductTypes"] = await _dbService.GetProductTypes();
        ViewData["Manufacturers"] = await _dbService.GetManufacturers();
        ViewData["Units"] = await _dbService.GetUnitsOfMeasure();
    }
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _dbService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        await PopulateDropdowns();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }
        Console.WriteLine("Edit POST request received for product Id: " + id);
        Console.WriteLine($"Product data received: Name={product.Name}, Description={product.Description}, Price={product.Price}, Quantity={product.Quantity}, ProductTypeId={product.ProductTypeId}, ManufacturerId={product.ManufacturerId}, UnitOfMeasureId={product.UnitOfMeasureId}");

        
            var success = await _dbService.UpdateProduct(product);

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "An error occurred while updating the product.";
        


        await PopulateDropdowns();
        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _dbService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        var success = await _dbService.DeleteProduct(id);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = "An error occurred while deleting the product.";
        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        Console.WriteLine("Create POST request received");
        Console.WriteLine($"Product data received: Name={product.Name}, Description={product.Description}, Price={product.Price}, Quantity={product.Quantity}, ProductTypeId={product.ProductTypeId}, ManufacturerId={product.ManufacturerId}, UnitOfMeasureId={product.UnitOfMeasureId}");

                await _dbService.AddProduct(product);
                return RedirectToAction(nameof(Index));
           
        await PopulateDropdowns();
        return View(product);
    }

}
