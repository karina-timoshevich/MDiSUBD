using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using LabRab6_MDiSUBD_Timoshevich.Models;

namespace LabRab6_MDiSUBD_Timoshevich.Controllers;

public class AdminController : Controller
{
    private readonly DbService _dbService;

    public AdminController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _dbService.GetAllClientsAsync();
        return View(clients);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = await _dbService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id)
        {
            return BadRequest();
        }

        var success = await _dbService.UpdateClientAsync(id, client);

        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = "An error occurred while updating the client.";
        return View(client);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var client = await _dbService.GetClientByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        var success = await _dbService.DeleteClientAsync(id);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = "An error occurred while deleting the client.";
        return View(client);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Client client)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _dbService.AddClient(client);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message; // В случае ошибки выводим сообщение
            }
        }

        return View(client);
    }

}
