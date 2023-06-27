using System.Net;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace LearnIOAPI.HttpResponses;

public static class CourseExceptions
{
    public static readonly ObjectResult Added = new(
        new { Response = "Course successfully added!" })
    {
        StatusCode = StatusCodes.Status200OK
    };
    
    public static readonly ObjectResult AlreadySigned = new(
        new { Response = "You are already signed for course with this language" })
    {
        StatusCode = StatusCodes.Status403Forbidden
    };
    
}