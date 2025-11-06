using PortHub.Api.Models;
using PortHub.Api.Context;
using PortHub.Api.Interfaces;
using PortHub.Api.Data;
//using Microsoft.EntityFrameworkCore;

namespace PortHub.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context; 

    public UserService(AppDbContext context)
    {_context = context;}

    public User AddUser(User user)
    {
        _context.Users.AddUser(user);
        _context.SaveChanges();
        return user;
    }

    public User? GetByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public bool DeleteUser(int id)
    {
        var user = GetById(id);
        if (user == null) return false;
        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }

    public List<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public User? GetById(int id)
    {
        return _context.Users.Find(id);
    }

    public User? UpdateUser(User user, int id)
    {
        var existUser = GetById(id);
        if(existUser == null) return null;
        
        existUser.Username = user.Username;
        existUser.PasswordHash = user.PasswordHash;

        _context.SaveChanges();
        return existUser;
    }
}