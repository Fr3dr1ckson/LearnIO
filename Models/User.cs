using System.ComponentModel.DataAnnotations;

namespace LearnIOAPI.Models;

public class User
{
    [Key, Required]
    public int telegramID { get; set; }
    
    public string Name { get; set; }
    
    public List<Course>? Courses { get; set; }
}