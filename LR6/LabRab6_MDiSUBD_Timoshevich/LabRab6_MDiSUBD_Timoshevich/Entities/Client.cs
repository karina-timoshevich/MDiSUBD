namespace LabRab6_MDiSUBD_Timoshevich.Entities;

public class Client
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}
