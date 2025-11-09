using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;

namespace PortHub.Api.Services;

public class AirlineService : IAirlineService
{
    private readonly AppDbContext _context;

    public AirlineService(AppDbContext context)
    {
        _context = context;
    }

    public List<Airline> GetAll()
    {
        return _context.Airlines.ToList();
    }

    public Airline GetById(int id)
    {
        return _context.Airlines
            .FirstOrDefault(a => a.Id == id);
    }

    public Airline GetByApiKey(string apiKey)
    {
        return _context.Airlines.FirstOrDefault(a => a.ApiKey == apiKey);
    }

    public Airline Add(Airline airline)
    {
        // Generar API Key única para cada aerolinea registrada 
        airline.ApiKey = GenerateApiKey(airline.Code);
        
        _context.Airlines.Add(airline);
        _context.SaveChanges();
        return airline;
    }

    public Airline Update(Airline airline, int id)
    {
        var existing = _context.Airlines.Find(id);
        if (existing == null)
            throw new KeyNotFoundException($"Airline con id {id} no se encontro.");

        existing.Name = airline.Name;
        existing.Code = airline.Code;
        existing.Country = airline.Country;
        existing.BaseAddress = airline.BaseAddress;
        existing.ApiUrl = airline.ApiUrl;

        _context.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = _context.Airlines.Find(id);
        if (existing == null) return false;
        
        _context.Airlines.Remove(existing);
        _context.SaveChanges();
        return true;
    }


    //Es el encargado de generar una API key unica para cada aerolinea
    private string GenerateApiKey(string code)
    {
        //Lo que hace es que la API key tenga el formato: CODE_KEY_XXXXXXXXXXXXXX
        //Lo genera en base a su codigo de aerolinea y un GUID unico
        var guid = Guid.NewGuid().ToString("N").ToUpper();
        return $"{code}_KEY_{guid.Substring(0, 24)}";
    }
}