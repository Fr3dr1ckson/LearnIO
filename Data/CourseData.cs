namespace LearnIOAPI.Data;

public class CourseData
{
    public string Language { get; set; }
    
    public double Progress { get; set; }
    
    public List<RoutineData>? Routines { get; set; }
}