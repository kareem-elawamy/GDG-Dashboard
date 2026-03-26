namespace GDG_DashBoard.DAL.Models;

public class Project : BaseEntity
{
    public Guid UserProfileId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public string? Period { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? Url { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}