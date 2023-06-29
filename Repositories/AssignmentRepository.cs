using LearnIOAPI.Data;
using LearnIOAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static LearnIOAPI.HttpResponses.GeneralResponses;
using Microsoft.EntityFrameworkCore;

namespace LearnIOAPI.Repositories;

public class AssignmentRepository
{
    private readonly ApplicationContext db;

    public AssignmentRepository(ApplicationContext db)
    {
        this.db = db;
    }
    public async Task<List<Assignment>?> Get(Routine routineData)
    {
        var routine = await  db.Routines.FindAsync(routineData);
        return routine.Assignments.ToList();
    }

    public async Task<List<Assignment>?> GetAll(Course courseData)
    {
        var course = await db.Courses.FindAsync(courseData);
        List<Assignment> res = new();
        foreach (var routine in course.Routines)
        {
            res.AddRange(routine.Assignments);
        }

        return res;
    }

    public async Task<ObjectResult> Create(int userId, string language, string theme, AssignmentData data)
    {
        var course = db.Courses.Where(course => course.UserId == userId && course.Language == language).ToList()[0];

        var Routines = new Routine
        {
            Completed = false,
            Mark = 0,
            Assignments = db.Assignments.Select(a => new Assignment
            {
                Answers = data.Answers,
                Audios = data.Audios.Select(a => new Audio{Data = a.Data}).ToList(),
                Questions = data.Questions,
                Task = data.Task,
                Images = a.Images.Select(image => new Image
                {
                    ImageName = image.ImageName,
                    ImageUri = image.ImageUri
                }).ToList(),
            }).ToList(),
        };
        course.Routines.Add(Routines);
        db.Courses.Update(course);
        
        await db.SaveChangesAsync();
        return new ObjectResult(
            new {Response = "You are already signed for course with this language"})
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    public async Task<ObjectResult> Update(AssignmentData data, int routineId)
    {
        var routine = await db.Routines.FindAsync(routineId);
        try
        {
            db.Assignments.Update(new Assignment
            {
                Answers = data.Answers,
                Audios = data.Audios.Select(a =>new Audio
                {
                    Data = a.Data
                }).ToList(),
                Images = data.Images.Select(i => new Image
                {
                    ImageName = i.ImageName,
                    ImageUri = i.ImageUri
                }).ToList(),
                Task = data.Task,
                Routine = routine
            });
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("assignment");
        }
        await db.SaveChangesAsync();
        return Updated("assignment");
    }
    public async Task<ObjectResult> Delete(Assignment data)
    {
        try
        {
            db.Assignments.Remove(data);
        }
        catch (Exception e)
        {
            Console.Write(e);
            return CannotBeDeleted("assignment");
        }
        await db.SaveChangesAsync();
        return Deleted("assignment");
    }
}