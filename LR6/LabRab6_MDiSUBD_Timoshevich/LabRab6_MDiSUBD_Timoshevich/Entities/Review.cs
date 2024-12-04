namespace LabRab6_MDiSUBD_Timoshevich.Entities;

public class Review
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
}