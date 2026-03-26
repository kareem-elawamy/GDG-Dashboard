namespace GDG_DashBoard.DAL.Models;

public class CommunityGroup : BaseEntity
{
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string JoinCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
    
    public required Guid InstructorId { get; set; }
    public Guid? RoadmapId { get; set; }
    
    public ApplicationUser Instructor { get; set; } = null!;
    public Roadmap? Roadmap { get; set; }
    
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
