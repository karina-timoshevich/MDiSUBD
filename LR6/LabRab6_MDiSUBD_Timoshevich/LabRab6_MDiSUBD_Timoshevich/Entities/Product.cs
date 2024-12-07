namespace LabRab6_MDiSUBD_Timoshevich.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ProductType { get; set; }  // Название типа продукта
    public string ManufacturerName { get; set; }  // Название производителя
    public string UnitOfMeasure { get; set; }  // Единица измерения
}
