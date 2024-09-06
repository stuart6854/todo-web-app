using System.ComponentModel.DataAnnotations;

namespace Domain;

public class ProjectModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid OwningUserId { get; set; }
}