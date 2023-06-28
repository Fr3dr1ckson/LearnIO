using LearnIOAPI.Data;
using LearnIOAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LearnIOAPI.HttpResponses.GeneralResponses;

namespace LearnIOAPI.Repositories;

public class CourseRepository
{
    private readonly ApplicationContext db;

    public CourseRepository(ApplicationContext db)
    {
        this.db = db;
    }
    public async Task<Course?> Get(int id)
    {
        return await db.Courses.SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> Create(int userId, string language)
    {
        var user = await db.Users.FindAsync(userId);
        if (user == null) return false;
        user.Courses.Add(new Course
        {
            Language = language
        });
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<ObjectResult> Update(Course data)
    {
        try
        {
            db.Courses.Update(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("course");
        }
        await db.SaveChangesAsync();
        return Updated("course");
    }
    public async Task<ObjectResult> Delete(Course data)
    {
        try
        {
            db.Courses.Remove(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("course");
        }
        await db.SaveChangesAsync();
        return Deleted("course");
    }
}