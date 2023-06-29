using LearnIOAPI.Data;
using LearnIOAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnIOAPI.Repositories;

public class AudioRepository
{
    private readonly ApplicationContext db;

    public AudioRepository(ApplicationContext db)
    {
        this.db = db;
    }
    
    
    public async Task<Audio?> GetById(int id)
    {
        return await db.Audios.FindAsync(id);
    }
    public async Task<Audio?> GetByAssignment(Assignment data)
    {
        return await db.Audios.FindAsync(data);
    }


    public async Task<bool> Create(Assignment assignment, AudioData audio)
    {
        try
        {
            var newAudio = new Audio
            {
                Data = audio.Data,
                Assignment = assignment
            };
            db.Audios.Add(newAudio);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        await db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> Create(AssignmentData assignmentData, AudioData audio)
    {
        
        try
        {
            var assignment = await db.Assignments.FindAsync(assignmentData);
            var newAudio = new Audio
            {
                Data = audio.Data,
                Assignment = assignment 
            };
            db.Audios.Add(newAudio);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> Update(AudioData audio)
    {
        try
        {
            db.Audios.Update(new Audio
            {
                Data = audio.Data
            });
        }
        catch (Exception e)
        {
            Console.Write(e);
            return false;
        }
        await db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> Delete(AudioData audio)
    {
        try
        {
            db.Audios.Remove(new Audio{Data = audio.Data});
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