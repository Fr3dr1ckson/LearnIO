namespace LearnIOAPI.Models;

public class Audio
{
    public int Id { get; set; }
    public byte[] Data { get; set; }
    public int AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
}