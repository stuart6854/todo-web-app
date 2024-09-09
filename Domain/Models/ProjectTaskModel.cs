using System.ComponentModel.DataAnnotations;

namespace Domain;

public class ProjectTaskModel
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid OwningProjectId { get; set; }
}