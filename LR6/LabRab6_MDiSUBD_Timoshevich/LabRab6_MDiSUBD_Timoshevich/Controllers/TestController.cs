using LabRab6_MDiSUBD_Timoshevich.Services;
using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Entities;  

namespace LabRab6_MDiSUBD_Timoshevich.Controllers
{
    public class TestController : Controller
    {
        private readonly DbService _dbService;

        public TestController(DbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _dbService.GetAllProducts();
            ViewBag.Products = products;
            return View();
        }
    }
}