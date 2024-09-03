namespace Domain;

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}