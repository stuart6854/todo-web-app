namespace TodoWebApp.Models.Entities;

public class TaskModel
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreationDate { get; set; }
}