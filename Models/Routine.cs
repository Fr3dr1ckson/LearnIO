using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class Routine
{
    [Key, Required]
    public int Id { get; set; }
    
    public List<Image> Images { get; set; }
    
    public string Theme { get; set; }
    
    public string AudioTaskText { get; set; }
    
    public int Mark { get; set; }
    
    public int CourseId { get; set; }
    
    public Course Course { get; set; }
}