namespace Kolokwium1.Models.DTOs;

public class AddVisitDTO
{
    public int visitId { get; set; }
    public int clientId { get; set; }
    public String mechanicLicenceNumber { get; set; }
    public List<ServiceDTO> services { get; set; }
    
}
public class ServiceDTO
{
    public string serviceName { get; set; }
    public decimal serviceFee { get; set; }
}