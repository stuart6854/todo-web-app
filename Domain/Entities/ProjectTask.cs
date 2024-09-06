using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsComplete { get; set; }

    public Guid ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
}