namespace GDG_DashBoard.BLL.Dtos.Member;

public class MemberDashboardDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfessionalSummary { get; set; }
    public int ProfileCompletenessPercentage { get; set; }
    
    public List<ActiveGroupDto> ActiveGroups { get; set; } = new();
    public List<ActiveRoadmapDto> ActiveRoadmaps { get; set; } = new();

    public GDG_DashBoard.BLL.ViewModels.Member.MemberProfileViewModel ProfileDetails { get; set; } = new();
}

public class ActiveGroupDto
{
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RoadmapTitle { get; set; }
}

public class ActiveRoadmapDto
{
    public Guid RoadmapId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}
