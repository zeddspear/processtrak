using System.ComponentModel.DataAnnotations;

public class BaseEntity
{
    [Key]
    public Guid id { get; set; } = Guid.NewGuid();
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public DateTime? updatedAt { get; set; }
    public DateTime? deletedAt { get; set; }
}
