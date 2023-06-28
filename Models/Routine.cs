using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class Routine
{
    [Key, Required]
    public int Id { get; set; }
    
    public string Theme { get; set; }
    
    public List<Assignment>? Assignments { get; set; }
    
    public int Mark { get; set; }
    
    public bool? Completed { get; set; }
    
    public int CourseId { get; set; }
    
    public Course Course { get; set; }
}