namespace GDG_DashBoard.DAL.Models;

public class Education : BaseEntity
{
    public Guid UserProfileId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Degree { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string University { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}