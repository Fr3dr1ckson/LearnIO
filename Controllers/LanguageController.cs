using System.Text.RegularExpressions;
using static LearnIOAPI.HttpResponses.User.UserExceptions;
using LearnIOAPI.Data;
using LearnIOAPI.HttpResponses;
using LearnIOAPI.HttpResponses.User;
using LearnIOAPI.Models;
using LearnIOAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;

namespace LearnIOAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LanguageController: Controller
{ 
    private readonly ApplicationContext db;

    private readonly UserRepository UserRepository;
    
    public LanguageController(ApplicationContext db)
    {
        this.db = db;
        UserRepository = new UserRepository(db);
    }
    
    private readonly string OpenAiKey;
    [HttpPost("register")]
    public async Task<ObjectResult> Register(UserData userCreds)
    {
        if (await db.Users.FindAsync(userCreds.telegramID) != null)
        {
            return AlreadyExists;
        }
        db.Users.Add(new User
        {
            telegramID = userCreds.telegramID,
            Name = userCreds.Name,
        });
        await db.SaveChangesAsync();

        return UserExceptions.Created;
    }
    
    [HttpGet("GetUser")]
    public async Task<ObjectResult> GetUser(int id)
    {
        var user = await UserRepository.Get(id);
        return user == null ? NonExistent : Found(user.Name);
    }
    [HttpPost("selectLanguage")]
    public async Task<ObjectResult> ChooseLanguage(int id, CourseData data)
    {
        var user = await db.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.telegramID == id);
        if (user == null)
        {
            return NonExistent;
        }

        if (user.Courses.Find(c => c.Language == data.Language) != null) return CourseExceptions.AlreadySigned;
        var course = new Course
        {
            Progress = data.Progress,
            Language = data.Language,
            Routines = data.Routines.Select(routine => new Routine
            {
                AudioTaskText = routine.AudioTaskText,
                Theme = routine.Theme,
                Mark = routine.Mark,
                Images = routine.Pictures.Select(img => new Image
                {
                    ImageName = img.ImageName,
                    ImageUri = img.ImageName
                }).ToList()
            }).ToList()
        };


        user.Courses.Add(course);
        db.Users.Update(user);
        await db.SaveChangesAsync();

        return CourseExceptions.Added;
    }

    [HttpDelete("unregister")]
    public async Task<ObjectResult> UnRegister(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NonExistent;
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Deleted;
    }

    [HttpPost("AddRoutine")]
    public async Task<ObjectResult> SendRoutine(int id, string language, RoutineData routineData)
    {
        var course = await db.Courses.Include(course => course.Routines).FirstOrDefaultAsync(c => c.UserId == id && c.Language == language);
        course.Routines.Add(new Routine
        {
            Theme = routineData.Theme,
            AudioTaskText = routineData.AudioTaskText,
            Mark = routineData.Mark
        });
        db.Courses.Update(course);
        await db.SaveChangesAsync();
        return CourseExceptions.Added;
    }
    [HttpGet("routine")]
    public async Task<string> GetRoutine(int id, string fromLanguage, string toLanguage, string level)
    {   
        var course = await db.Courses.FirstOrDefaultAsync(c => c.UserId == id && c.Language == fromLanguage);

        string usedThemes;

        if (course != null)
        {
                usedThemes = string.Join(", ", await db.Routines
                .Where(routine => routine.CourseId == course.Id)
                .Select(routine => routine.Theme)
                .ToListAsync());
        }
        else
        {
            return "No course found";
        }
        var tasksText = await System.IO.File.ReadAllTextAsync("C:\\Users\\onarg\\RiderProjects\\LearnIOAPI\\NewFile1.txt");
        
        var replacements = new Dictionary<string, string> 
        {
            {@"\[Level\]", level},
            {@"\[Themes\]", usedThemes},
            {@"\[fromLanguage\]", fromLanguage},
            {@"\[toLanguage\]", toLanguage}
        };
        foreach(var pair in replacements)
        {
            var regex = new Regex(pair.Key);
            tasksText = regex.Replace(tasksText, pair.Value);
        }
        var api = new OpenAIAPI("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");
        var chat = api.Chat.CreateConversation();
        chat.AppendSystemMessage(tasksText);
        var response = await chat.GetResponseFromChatbotAsync();
        
        return response;
    }
}