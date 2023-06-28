using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class Image
{
    [Key, Required]
    public int Id { get; set; }
    
    public string ImageName { get; set; }
    
    public string ImageUri { get; set; }
    
    public int AssignmentId { get; set; }
    
    public Assignment Assignment { get; set; }
}