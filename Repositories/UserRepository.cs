using LearnIOAPI.HttpResponses;
using LearnIOAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LearnIOAPI.HttpResponses.User.UserExceptions;

namespace LearnIOAPI.Repositories;

public class UserRepository
{
    private readonly ApplicationContext db;

    public UserRepository(ApplicationContext db)
    {
        this.db = db;
    }
    public async Task<User?> Get(int id)
    {
        return await db.Users.SingleOrDefaultAsync(u => u.telegramID == id);
    }


    public async Task<ObjectResult> Create(User data)
    {
        await db.Users.AddAsync(new User
        {
            Name = data.Name,
            telegramID = data.telegramID,
            Courses = data.Courses
        });
        await db.SaveChangesAsync();
        return Created;
    }
    public async Task<ObjectResult> Update(User data)
    {
        try
        {
            db.Users.Update(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return UserCannotBeDeleted;
        }
        await db.SaveChangesAsync();
        return Updated;
    }
    public async Task<ObjectResult> Delete(User data)
    {
        try
        {
            db.Users.Remove(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return UserCannotBeDeleted;
        }
        await db.SaveChangesAsync();
        return Deleted;
    }
}