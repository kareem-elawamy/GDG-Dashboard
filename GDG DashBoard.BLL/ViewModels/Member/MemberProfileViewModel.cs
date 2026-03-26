namespace GDG_DashBoard.BLL.ViewModels.Member;

public class MemberProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfessionalSummary { get; set; }
    public string? Location { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    
    // AI CV fields mapped structurally
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public List<ExperienceViewModel> Experiences { get; set; } = new();
    public List<EducationViewModel> Educations { get; set; } = new();
    public List<ProjectViewModel> Projects { get; set; } = new();
    
    public List<string> TechnicalSkills { get; set; } = new();
    public List<string> SoftSkills { get; set; } = new();
}

public class ExperienceViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Period { get; set; }
    public string? Description { get; set; }
}

public class EducationViewModel
{
    public string Degree { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public string? Year { get; set; }
}

public class ProjectViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Period { get; set; }
    public string? Url { get; set; }
}
