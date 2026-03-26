namespace GDG_DashBoard.DAL.Models;

public class GroupMember : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid MemberId { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public CommunityGroup Group { get; set; } = null!;
    public ApplicationUser Member { get; set; } = null!;
}
