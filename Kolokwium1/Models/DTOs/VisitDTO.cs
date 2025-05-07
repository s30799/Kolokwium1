namespace Kolokwium1.Models.DTOs;

public class VisitDTO
{
    public DateTime Date { get; set; }
    public List<ClientDTO> Clietns { get; set; }
    public List<MechanicDTO> Mechanics { get; set; }
    public List<visitServicesDTO> Services { get; set; } 
}
public class ClientDTO
{
    public string firstName { get; set; }
    public string lastSurname { get; set; }
    public DateTime dateOfBirth { get; set; }
}
public class MechanicDTO
{
    public int mechanicId { get; set; }
    public string licenceNumber { get; set; }
}  
public class visitServicesDTO
{
    public string name { get; set; }
    public decimal serviceFee { get; set; }
}