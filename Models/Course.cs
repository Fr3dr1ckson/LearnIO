using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class Course
{
    [Key, Required]
    public int Id { get; set; }
    
    public string Language { get; set; }
    
    public double Progress { get; set; }
    
    public List<Routine>? Routines { get; set; }
    
    public int UserId { get; set; }
    
    public User User { get; set; }
}