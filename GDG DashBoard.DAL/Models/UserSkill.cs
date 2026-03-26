using GDG_DashBoard.DAL.Eums;

namespace GDG_DashBoard.DAL.Models;

public class UserSkill : BaseEntity
{
    public Guid UserProfileId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Name { get; set; }
    public SkillType Type { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}