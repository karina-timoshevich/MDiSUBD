using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace LabRab6_MDiSUBD_Timoshevich.Models;

public class OrdersViewModel
{
    public List<OrderViewModel> Orders { get; set; }
    public List<SelectListItem> OrderStatuses { get; set; }
}

public class OrderViewModel
{
    public Orders Order { get; set; }
    public string SelectedStatus { get; set; }  
}
