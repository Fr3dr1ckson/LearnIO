using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class Image
{
    [Key, Required]
    public int Id { get; set; }
    
    public string ImageName { get; set; }
    
    public string ImageUri { get; set; }
    
    public int RoutineId { get; set; }
    
    public Routine Routine { get; set; }
}