using System; 
using PortHub.Api.Models;

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
}