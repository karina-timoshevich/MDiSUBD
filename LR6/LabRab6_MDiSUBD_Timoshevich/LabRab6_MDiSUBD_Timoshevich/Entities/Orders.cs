namespace LabRab6_MDiSUBD_Timoshevich.Entities;

public class Orders
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int? PromoCodeId { get; set; }
    public int? PickupLocationId { get; set; }
    public string Status { get; set; }
}