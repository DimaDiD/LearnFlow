namespace LearnFlow.Search.Domain.Entities;

public class CourseDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int EnrollmentCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
}