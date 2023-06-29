using Microsoft.AspNetCore.Mvc;

namespace LearnIOAPI.HttpResponses;

public class GeneralResponses
{

    public static ObjectResult Found(string entity)
    {
        return new ObjectResult(
            new { Response = $"{entity}" })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    public static ObjectResult Updated(string entity)
    {
        return new ObjectResult(
            new { Response = $"{entity} successfully updated!" })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    
    public static ObjectResult CannotBeUpdated(string entity)
    {
        return new ObjectResult(
            new { Response = $"{entity} cannot be updated!" })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
    public static ObjectResult AlreadyExists(string entity)
    {
        return new ObjectResult(
            new { error = "Forbidden" , Response = $"{entity} with such id already exists in database"})
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
    
    public static ObjectResult Created(string entity)
    {
        return new ObjectResult(
            new { Response = $"{entity} successfully created!" })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    public static ObjectResult NonExistent(string entity)
    {
        return new ObjectResult (
            new { Response = $"{entity} with such id or data does not exist in database"})
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
    
    public static ObjectResult Deleted(string entity)
    {
        return new ObjectResult(
            new { Response = $"{entity} deleted successfully!" })
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
    
    public static ObjectResult CannotBeDeleted(string entity)
    {
        return new ObjectResult(
            new { Response = $"Cannot delete this {entity}!" })
        {
            StatusCode = StatusCodes.Status403Forbidden 
        };
    }
}