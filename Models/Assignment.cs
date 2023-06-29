using System.ComponentModel.DataAnnotations;
namespace LearnIOAPI.Models;

public class Assignment
{
    [Key, Required]
    public int Id { get; set; }
    
    public string[]? Answers { get; set; }
    
    public string[]? Questions { get; set; }
    
    public string? Task { get; set; }
    
    public List<Audio>? Audios { get; set; }
    
    public List<Image>? Images { get; set; }
    
    public int RoutineId { get; set; }
    
    public Routine Routine { get; set; }
}