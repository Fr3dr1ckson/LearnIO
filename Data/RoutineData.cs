namespace LearnIOAPI.Data;

public class RoutineData
{
    public string Theme { get; set; }
    
    public List<AssignmentData>? Assignments { get; set; }
    
    public bool Completed { get; set; }
    
    public int Mark { get; set; }
}