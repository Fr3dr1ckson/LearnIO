using Microsoft.AspNetCore.Mvc;

namespace LearnIOAPI.HttpResponses.User;

public static class UserExceptions
{
    public static ObjectResult Found(string username)
    {
        return new ObjectResult(
            new { Response = $"{username}" })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static readonly ObjectResult Updated = new(
        new { Response = "User successfully updated!" })
    {
        StatusCode = StatusCodes.Status200OK
    };
    
    public static readonly ObjectResult AlreadyExists = new(
        new { error = "Forbidden" , Response = "User with such id already exists in database"})
    {
        StatusCode = StatusCodes.Status403Forbidden
    };
    public static readonly ObjectResult Created = new(
        new { Response = "User successfully created!" })
    {
        StatusCode = StatusCodes.Status200OK
    };

    public static readonly ObjectResult NonExistent = new(
        new { error = "Forbidden" , Response = "User with such id does not exist in database"})
    {
        StatusCode = StatusCodes.Status403Forbidden
    };
    
    public static readonly ObjectResult Deleted = new(
        new { Response = "Deleted user successfully!" })
    {
        StatusCode = StatusCodes.Status200OK
    };
    
    public static readonly ObjectResult UserCannotBeDeleted = new(
        new { Response = "Cannot delete this user!" })
    {
        StatusCode = StatusCodes.Status403Forbidden 
    };
}