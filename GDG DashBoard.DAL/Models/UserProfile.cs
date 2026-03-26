namespace GDG_DashBoard.DAL.Models;

public class UserProfile : BaseEntity
{
    public required Guid UserId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string FullName { get; set; }
    public string? ProfessionalSummary { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? Location { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? ResumeFileUrl { get; set; }
    public bool IsVerified { get; set; } = false;

    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? ContactEmail { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    public string? Phone { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? GitHubUrl { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? LinkedInUrl { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Education> Educations { get; set; } = new List<Education>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<UserSkill> Skills { get; set; } = new List<UserSkill>();
}