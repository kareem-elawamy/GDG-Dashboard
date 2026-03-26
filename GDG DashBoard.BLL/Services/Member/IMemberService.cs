using GDG_DashBoard.BLL.Dtos.Member;
using GDG_DashBoard.BLL.ViewModels.Member;

namespace GDG_DashBoard.BLL.Services.Member;

public interface IMemberService
{
    Task<MemberDashboardDto?> GetMemberDashboardAsync(Guid userId);
    Task<MemberProfileViewModel> GetProfileForEditAsync(Guid userId);
    Task UpdateMemberProfileAsync(Guid userId, MemberProfileViewModel model);
    Task<RoadmapDetailsForMemberDto?> GetRoadmapDetailsForMemberAsync(Guid roadmapId, Guid userId);
    Task<ToggleProgressResultDto> ToggleNodeProgressAsync(Guid userId, Guid levelId);
}
