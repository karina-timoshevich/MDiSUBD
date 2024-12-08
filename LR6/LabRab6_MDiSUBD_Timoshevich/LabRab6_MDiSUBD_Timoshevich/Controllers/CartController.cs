using Microsoft.AspNetCore.Mvc;
using LabRab6_MDiSUBD_Timoshevich.Services;
using LabRab6_MDiSUBD_Timoshevich.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CartController : Controller
{
    private readonly DbService _dbService;

    public CartController(DbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to view your cart.";
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _dbService.GetCartItemsByClientId(clientId.Value);
        var pickupLocations = await _dbService.GetAllPickupLocations();
        var promoCodes = await _dbService.GetAllPromoCodes();
        ViewBag.PickupLocations = new SelectList(pickupLocations, "Id", "Name");
        ViewBag.PromoCodes = new SelectList(promoCodes, "Id", "Code"); 

        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(int productId, int quantity, int? promoCodeId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to update your cart.";
            return RedirectToAction("Login", "Account");
        }
        Console.WriteLine($"ProductId(UpdateCart): {productId}, Quantity(UpdateCart): {quantity}, ClientId(UpdateCart): {clientId}");

        var updated = await _dbService.UpdateCartItemQuantity(clientId.Value, productId, quantity);
        if (!updated)
        {
            TempData["ErrorMessage"] = "Unable to update the item quantity.";
            return RedirectToAction("Index");
        }
        var cartItems = await _dbService.GetCartItemsByClientId(clientId.Value);
        decimal totalPrice = cartItems.Sum(item => item.Quantity * item.Price);

        // Применяем промокод, если он есть
        if (promoCodeId.HasValue)
        {
            var discount = await _dbService.GetPromoCodeDiscount(promoCodeId.Value);
            Console.WriteLine($"Promo Code ID: {promoCodeId.Value}, Discount: {discount}%");
            totalPrice -= totalPrice * (discount / 100);  // Скидка как процент
            Console.WriteLine($"Total price after discount: {totalPrice}");
        }

        // Обновляем цену в корзине
        await _dbService.UpdateCartTotalPrice(clientId.Value, totalPrice);

        TempData["SuccessMessage"] = "Your cart has been updated.";
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> ApplyPromoCode(int? promoCodeId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to apply a promo code.";
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await _dbService.GetCartItemsByClientId(clientId.Value);
        decimal totalPrice = cartItems.Sum(item => item.Quantity * item.Price);

        if (promoCodeId.HasValue)
        {
            var discount = await _dbService.GetPromoCodeDiscount(promoCodeId.Value);
            totalPrice -= totalPrice * (discount / 100); // Apply discount
        }

        // Обновление цены в базе данных
        await _dbService.UpdateCartTotalPrice(clientId.Value, totalPrice);

        ViewBag.TotalPrice = totalPrice;  // Передаем обновленную цену в View
        TempData["SuccessMessage"] = "Promo code applied successfully!";
        return RedirectToAction("Index");
    }



    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to remove items from your cart.";
            return RedirectToAction("Login", "Account");
        }

        await _dbService.RemoveFromCart( productId, clientId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int pickupLocationId, int? promoCodeId)
    {
        var clientId = HttpContext.Session.GetInt32("ClientId");
        if (clientId == null)
        {
            TempData["ErrorMessage"] = "You need to be logged in to place an order.";
            return RedirectToAction("Login", "Account");
        }

        var totalPrice = await _dbService.GetCartTotalPrice(clientId.Value);
        if (promoCodeId.HasValue)
        {
            // Получаем скидку по промокоду
            var discount = await _dbService.GetPromoCodeDiscount(promoCodeId.Value);
            totalPrice -= totalPrice * (discount / 100);  // Скидка как процент
        }

        await _dbService.CreateOrder(clientId.Value, pickupLocationId, promoCodeId, totalPrice);

        TempData["SuccessMessage"] = "Your order has been placed successfully.";
        return RedirectToAction("Index", "Cart");
    }


}
