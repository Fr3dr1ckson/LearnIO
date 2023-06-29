using LearnIOAPI.Data;
using LearnIOAPI.Models;
using LearnIOAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LearnIOAPI.HttpResponses.GeneralResponses;

namespace LearnIOAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoutineController: Controller
{
    private readonly ApplicationContext _db;
    
    private readonly RoutineRepository _routineRepository;
    
    public RoutineController(ApplicationContext db)
    {
        _db = db;
        _routineRepository = new RoutineRepository(db);
    }

    [HttpGet("getRoutine")]
    public ActionResult<Assignment> getRoutine(int courseId)
    {
        var res = _db.Routines.Where(r => r.CourseId == courseId)
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
        return Ok(res);

    }
    
}