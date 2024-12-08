namespace LabRab6_MDiSUBD_Timoshevich.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int CartId { get; set; }
    
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}