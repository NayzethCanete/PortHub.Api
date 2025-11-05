using PortHub.Api.Models;

<<<<<<< HEAD
namespace PortHub.Api.Interface
{
    public interface IAirlineService
    {
        List<Airline> GetAll();
        Airline? GetById(int id);
        Airline Add(Airline airline);
        Airline Update(Airline airline, int id);
        bool Delete(int id);
    }
=======
namespace PortHub.Api.Interfaces;

public interface IAirlineService
{
    List<Airline> GetAll();
    Airline GetById(int id);
    Airline GetByApiKey(string apiKey);
    Airline Add(Airline airline);
    Airline Update(Airline airline, int id);
    bool Delete(int id);
>>>>>>> BD-setup
}