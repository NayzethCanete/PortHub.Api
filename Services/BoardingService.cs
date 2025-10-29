using System;
using System.Collections.Generic;
using System.Linq;
using PortHub.Api.Interfaces;
using PortHub.Api.Models;
using Microsoft.EntityFrameworkCore;
using PortHub.Api.Data;

namespace PortHub.Api.Services;

public class BoardingService : IBoardingService
{
    private readonly AppDbContext _context;

    public BoardingService(AppDbContext context)
    {
        _context = context;
    }

    public List<Boarding> GetAll()
    {
        return _context.Boardings
            .Include(b => b.Slot)
                .ThenInclude(s => s.Gate)
            .ToList();
    }

    public Boarding GetById(int id)
    {
        return _context.Boardings
            .Include(b => b.Slot)
                .ThenInclude(s => s.Gate)
            .FirstOrDefault(b => b.BoardingId == id);
    }

    public List<Boarding> GetBySlotId(int slotId)
    {
        return _context.Boardings
            .Where(b => b.SlotId == slotId)
            .ToList();
    }

    public Boarding Add(Boarding boarding)
    {
        _context.Boardings.Add(boarding);
        _context.SaveChanges();
        return GetById(boarding.BoardingId);
    }

    public Boarding Update(Boarding boarding, int id)
    {
        var existing = _context.Boardings.Find(id);
        if (existing == null)
            throw new KeyNotFoundException($"Boarding con id {id} no se encontro.");

        existing.TicketId = boarding.TicketId;
        existing.AccessTime = boarding.AccessTime;
        existing.Validation = boarding.Validation;
        existing.SlotId = boarding.SlotId;

        _context.SaveChanges();
        return GetById(id);
    }

    public bool Delete(int id)
    {
        var existing = _context.Boardings.Find(id);
        if (existing == null) return false;
        
        _context.Boardings.Remove(existing);
        _context.SaveChanges();
        return true;
    }
}