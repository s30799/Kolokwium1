using Kolokwium1.Models.DTOs;

namespace Kolokwium1.Services;

public interface IDbService
{
    Task<bool> DoesVisitExist(int id);
    Task <VisitDTO> GetVisit(int id);
    Task<int> AddVisit(AddVisitDTO visit);
    Task<bool> DoesClientExist(int id);
    Task<bool> DoesMechanicExist(string licenceNumber);
    Task<bool> DoesServiceExist(string name);
    
    
}