namespace Domain;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public User OwningUser { get; set; }
    public ICollection<Task> Tasks { get; }
}