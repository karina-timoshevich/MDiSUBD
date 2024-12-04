using LabRab6_MDiSUBD_Timoshevich.Services;
using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Entities;  // Используем пространство имен Entities

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
            // Получаем список продуктов
            var products = await _dbService.GetAllProducts();
            // Передаем продукты в представление через ViewBag
            ViewBag.Products = products;
            return View();
        }
    }
}