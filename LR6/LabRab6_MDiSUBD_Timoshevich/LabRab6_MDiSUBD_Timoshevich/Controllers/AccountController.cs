using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;

namespace LabRab6_MDiSUBD_Timoshevich.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbService _dbService;

        public AccountController(DbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.ErrorMessage = "Email and password are required.";
                return View();
            }

            var client = await _dbService.GetClientByEmail(model.Email);
            var employee = await _dbService.GetEmployeeByEmail(model.Email);

            if (client != null && client.Password == model.Password)
            {
                ViewBag.WelcomeMessage = $"Hello, Client {client.FirstName} {client.LastName} ({client.Email})";
                return RedirectToAction("Index", "Home");
            }
            else if (employee != null && employee.Password == model.Password)
            {
                ViewBag.WelcomeMessage = $"Hello, Employee {employee.FirstName} {employee.LastName} ({employee.Email})";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
            }

            return View();
        }
    }
}