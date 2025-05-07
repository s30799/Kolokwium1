using Kolokwium1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace Kolokwium1.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesVisitExist(int id)
    {
        var query = "SELECT 1 FROM Visit WHERE visit_id = @id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null;
    }

    public async Task<VisitDTO> GetVisit(int id)
    {
        var query = @"
            SELECT 
                v.visit_id AS VisitId,
                v.date AS VisitDate,
                c.first_name AS ClientFirstName,
                c.last_name AS ClientLastName,
                c.date_of_birth AS ClientDateOfBirth,
                m.mechanic_id AS MechanicId,
                m.licence_number AS MechanicLicenceNumber,
                s.name AS ServiceName,
                vs.service_fee AS ServiceFee
            FROM Visit v
            LEFT JOIN Client c ON v.client_id = c.client_id
            LEFT JOIN Mechanic m ON v.mechanic_id = m.mechanic_id
            LEFT JOIN Visit_Service vs ON v.visit_id = vs.visit_id
            LEFT JOIN Service s ON vs.service_id = s.service_id
            WHERE v.visit_id = @id;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        var originalVisitId = reader.GetOrdinal("VisitId");
        var originalVisitDate = reader.GetOrdinal("VisitDate");
        var originalClientFirstName = reader.GetOrdinal("ClientFirstName");
        var originalClientLastName = reader.GetOrdinal("ClientLastName");
        var originalClientDateOfBirth = reader.GetOrdinal("ClientDateOfBirth");
        var originalMechanicId = reader.GetOrdinal("MechanicId");
        var originalMechanicLicenceNumber = reader.GetOrdinal("MechanicLicenceNumber");
        var originalServiceName = reader.GetOrdinal("ServiceName");
        var originalServiceFee = reader.GetOrdinal("ServiceFee");

        VisitDTO visitDto = null;

        while (await reader.ReadAsync())
        {
            if (visitDto is not null)
            {
                visitDto.Clietns.Add(new ClientDTO
                {
                    firstName = reader.GetString(originalClientFirstName),
                    lastSurname = reader.GetString(originalClientLastName),
                    dateOfBirth = reader.GetDateTime(originalClientDateOfBirth)
                });
            }
            else
            {
                visitDto = new VisitDTO
                {
                    Date = reader.GetDateTime(originalVisitDate),
                    Clietns = new List<ClientDTO>
                    {
                        new ClientDTO
                        {
                            firstName = reader.GetString(originalClientFirstName),
                            lastSurname = reader.GetString(originalClientLastName),
                            dateOfBirth = reader.GetDateTime(originalClientDateOfBirth)
                        }
                    },
                    Mechanics = new List<MechanicDTO>
                    {
                        new MechanicDTO
                        {
                            mechanicId = reader.GetInt32(originalMechanicId),
                            licenceNumber = reader.GetString(originalMechanicLicenceNumber)
                        }
                    },
                    Services = new List<visitServicesDTO>
                    {
                        new visitServicesDTO
                        {
                            name = reader.GetString(originalServiceName),
                            serviceFee = reader.GetDecimal(originalServiceFee)
                        }
                    }
                };
            }
        }

        if (visitDto is null) throw new Exception("Visit not found");
        return visitDto;

    }

    public async Task<bool> DoesClientExist(int id)
    {
        var query = "SELECT 1 FROM Client WHERE client_id = @id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null;
    }

    public async Task<bool> DoesMechanicExist(string licenceNumber)
    {
        var query = "SELECT 1 FROM Mechanic WHERE licence_number = @licenceNumber";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@licenceNumber", licenceNumber);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null;
    }

    public async Task<bool> DoesServiceExist(string name)
    {
        var query = "SELECT 1 FROM Service WHERE name = @name";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@name", name);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        return res is not null;
    }

    public async Task<int> AddVisit(AddVisitDTO visit)
    {
        var query = @"
INSERT INTO Visit (client_id, mechanic_id, licence_number, date)
OUTPUT INSERTED.visit_id
VALUES (@clientId, @mechanicId, @mechanicLicenceNumber, @date);";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@clientId", visit.clientId);
        command.Parameters.AddWithValue("@mechanicId", visit.mechanicLicenceNumber);
        command.Parameters.AddWithValue("@mechanicLicenceNumber", visit.mechanicLicenceNumber);
        command.Parameters.AddWithValue("@date", DateTime.Now);
        await connection.OpenAsync();
        var res = await command.ExecuteScalarAsync();
        if (res is null) throw new Exception();
        return (int)res;
    }
}

