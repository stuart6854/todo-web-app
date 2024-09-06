using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid OwningUserId { get; set; }
    [ForeignKey("OwningUserId")]
    public virtual User OwningUser { get; set; }

    public ICollection<ProjectTask> Tasks { get; }
}