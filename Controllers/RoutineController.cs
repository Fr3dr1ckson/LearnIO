using LearnIOAPI.Data;
using LearnIOAPI.Models;
using LearnIOAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LearnIOAPI.HttpResponses.GeneralResponses;
using ILogger = Google.Apis.Logging.ILogger;

namespace LearnIOAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoutineController: Controller
{
    private readonly ApplicationContext _db;
    
    private readonly RoutineRepository _routineRepository;
    
    
    private readonly ILogger<RoutineController> _logger;
    
    public RoutineController(ApplicationContext db, ILogger<RoutineController> logger)
    {
        _db = db;
        _routineRepository = new RoutineRepository(db);
        _logger = logger;
    }

    [HttpGet("getRoutine")]
    public ActionResult<Assignment> GetIncomplete(int userId)
    {
        dynamic res;
        try
        {
            var courseId = _db.Courses.First(c => c.UserId == userId).Id;
            res = _db.Routines.Where(r => r.CourseId == courseId && r.Completed == false)
                .Select(c => c.Assignments.Select(a => new
                {
                    Audios = a.Audios.Select(audio => new
                    {
                        audio.Data,
                    }),
                    Images = a.Images.Select(image => new
                    {
                        image.ImageName,
                        image.ImageUri
                    }),
                    a.Questions,
                    a.Answers
                }));
        }
        catch (Exception e)
        {
            _logger.LogError("Error: {e}");
            throw;
        }
        
        return Ok(res.First());
    }
    
}