﻿using LearnIOAPI.Data;
using LearnIOAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LearnIOAPI.HttpResponses.GeneralResponses;

namespace LearnIOAPI.Repositories;

public class RoutineRepository
{
    
    private readonly ApplicationContext db;

    public RoutineRepository(ApplicationContext db)
    {
        this.db = db;
    }
    
    public async Task<Routine?> Get(int id)
    {
        return await db.Routines.SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ObjectResult> Create(int userId, string language,Routine data)
    {
        var course = db.Courses.Where(c => c.UserId == userId && c.Language == language)
            .Include(course => course.Routines).ToList().First();
        
        var routine = new Routine
        {
            Theme = data.Theme,
            Completed = false,
            Mark = 0,
            Assignments = data.Assignments,
            
        };
        course.Routines.Add(routine);
        await db.SaveChangesAsync();
        return Created("routine");
    }

    public async Task<bool> sendMark(int id, int mark)
    {
        var routine = await db.Routines.FindAsync(id);
        if (routine == null) return false;
        routine.Mark = mark;
        db.Routines.Update(routine);
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<ObjectResult> Update(Routine data)
    {
        try
        {
            db.Routines.Update(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("routine");
        }
        await db.SaveChangesAsync();
        return Updated("routine");
    }
    public async Task<ObjectResult> Delete(Routine data)
    {
        try
        {
            db.Routines.Remove(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("routine");
        }
        await db.SaveChangesAsync();
        return Deleted("routine");
    }
}