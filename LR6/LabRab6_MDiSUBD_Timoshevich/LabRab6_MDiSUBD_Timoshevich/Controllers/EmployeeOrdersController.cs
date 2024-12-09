using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using LabRab6_MDiSUBD_Timoshevich.Models;

namespace LabRab6_MDiSUBD_Timoshevich.Controllers
{
    public class EmployeeOrdersController : Controller
    {
        private readonly DbService _dbService;

        public EmployeeOrdersController(DbService dbService)
        {
            _dbService = dbService;
        }

        private bool IsEmployeeLoggedIn()
        {
            return HttpContext.Session.GetInt32("EmployeeId") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsEmployeeLoggedIn()) 
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _dbService.GetOrdersForWorkerAsync();

            // Создаем список для статусов
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "Shipped", Text = "Shipped" },
                new SelectListItem { Value = "Delivered", Text = "Delivered" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };

            var viewModel = new OrdersViewModel
            {
                Orders = orders,
                OrderStatuses = statuses
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string selectedStatus)
        {
            // Пример получения выбранного статуса
            var result = await _dbService.UpdateOrderStatusAsync(orderId, selectedStatus);

            if (result)
            {
                // Статус успешно обновлён
                return RedirectToAction("Index"); // Перенаправить на страницу со списком заказов
            }
            else
            {
                // Ошибка при обновлении статуса
                return View(); // Возвращаем на текущую страницу с ошибкой
            }
        }

    }
}
