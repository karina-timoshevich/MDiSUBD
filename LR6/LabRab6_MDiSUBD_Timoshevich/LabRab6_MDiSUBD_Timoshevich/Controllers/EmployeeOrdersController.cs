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

            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "Shipped", Text = "Shipped" },
                new SelectListItem { Value = "Delivered", Text = "Delivered" },
                new SelectListItem { Value = "Canceled", Text = "Canceled" }
            };
           
            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                Order = order,
                SelectedStatus = order.Status  
            }).ToList();

            var viewModel = new OrdersViewModel
            {
                Orders = orderViewModels,
                OrderStatuses = statuses
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string selectedStatus)
        {
            Console.WriteLine($"Received orderId: {orderId}, selectedStatus: {selectedStatus}");

            if (string.IsNullOrEmpty(selectedStatus))
            {
                ModelState.AddModelError("", "Please select a status.");
                return RedirectToAction("Index");
            }
            Console.WriteLine($"Updating OrderId: {orderId} with Status: {selectedStatus}");
            var result = await _dbService.UpdateOrderStatusAsync(orderId, selectedStatus);

            if (result)
            {
                return RedirectToAction("Index"); 
            }
            else
            {
                return View(); 
            }
        }


    }
}
