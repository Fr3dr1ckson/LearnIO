using LearnIOAPI.Data;
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


    public async Task<ObjectResult> Create(UserData data)
    {
        await db.Users.AddAsync(new User
        {
            Name = data.Name,
            MainLanguage = data.MainLanguage,
            telegramID = data.telegramID
        });
        await db.SaveChangesAsync();
        return Created;
    }

    public async Task<bool>  AddCourse(int id, int courseId)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return false;
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) return false;
        user.Courses.Add(course);
        await Update(user);
        return true;
    }
    public async Task<bool> Update(User data)
    {
        try
        {
            db.Users.Update(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return false;
        }
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> Delete(int id)
    {
        try
        {
            db.Users.Remove(await Get(id));
        }
        catch (Exception e)
        {
            Console.Write(e);
            return false;
        }
        await db.SaveChangesAsync();
        return true;
    }
}