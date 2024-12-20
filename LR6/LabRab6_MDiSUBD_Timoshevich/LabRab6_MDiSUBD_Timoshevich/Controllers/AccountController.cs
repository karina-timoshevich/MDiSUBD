﻿using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;
using Npgsql;

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
                HttpContext.Session.SetInt32("ClientId", client.Id);
                HttpContext.Session.SetString("UserRole", "Client");
                Console.WriteLine($"Hello, Client {client.FirstName} {client.LastName} ({client.Email})");
                return RedirectToAction("Index", "Home");
            }
            else if (employee != null && employee.Password == model.Password)
            {
                HttpContext.Session.SetInt32("EmployeeId", employee.Id);
                var employeeIdFromSession = HttpContext.Session.GetInt32("EmployeeId");
                Console.WriteLine($"EmployeeId from session: {employeeIdFromSession}");

               //await _dbService.SetEmployeeIdInSession(employee.Id);
              // await _dbService.CheckEmployeeIdInSession();
               var role = employee.PositionId == 8 ? "Admin" : "Worker";
               HttpContext.Session.SetString("UserRole", role);
               Console.WriteLine($"Hello, {role} {employee.FirstName} {employee.LastName} ({employee.Email}) position {employee.PositionId}");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
            }

            return View();
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());  
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View();
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            var existingClient = await _dbService.GetClientByEmail(model.Email);
            var existingEmployee = await _dbService.GetEmployeeByEmail(model.Email);

            if (existingClient != null || existingEmployee != null)
            {
                ViewBag.ErrorMessage = "Email is already taken.";
                return View();
            }

            var newClient = new Client
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth, 
                PhoneNumber = model.PhoneNumber, 
                Email = model.Email,
                Password = model.Password
            };

            await _dbService.AddClient(newClient); 

            return RedirectToAction("Login");
        }
        
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}