namespace LearnIOAPI.Data;

public class AssignmentData
{
    public string? Task { get; set; }
    
    public string[]? Questions { get; set; }
    
    public string[] Answers { get; set; }
    
    public List<byte[]>? Audio { get; set; }
    
    public List<ImageData>? Images { get; set; }
}