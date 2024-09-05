using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class ProjectModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid OwningUserId { get; set; }
}