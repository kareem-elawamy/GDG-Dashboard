namespace GDG_DashBoard.BLL.Dtos.Member;

public class ToggleProgressResultDto
{
    public bool Success { get; set; }
    public bool IsCompleted { get; set; }
    public int CompletedCount { get; set; }
    public int TotalLevels { get; set; }
    public int Percentage { get; set; }
}

public class RoadmapDetailsForMemberDto
{
    public DAL.Models.Roadmap Roadmap { get; set; } = null!;
    public HashSet<Guid> CompletedLevelIds { get; set; } = new();
    public int TotalLevels { get; set; }
    public int CompletedCount { get; set; }
    public bool IsEnrolled { get; set; }
}
