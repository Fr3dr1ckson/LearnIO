namespace LearnIOAPI.Data;

public class UserData
{
    public int telegramID { get; set; }
    
    public string Name { get; set; }
    
    public List<CourseData>? Courses { get; set; }
}