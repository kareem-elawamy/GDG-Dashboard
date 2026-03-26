using GDG_DashBoard.BLL.Dtos.Member;
using GDG_DashBoard.BLL.ViewModels.Member;
using GDG_DashBoard.BLL.Services.RoadmapServices;
using GDG_DashBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GDG_DashBoard.DAL.Repositores.GenericRepository;

namespace GDG_DashBoard.BLL.Services.Member;

public class MemberService : IMemberService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepositoryAsync<UserProfile> _profileRepo;
    private readonly IGenericRepositoryAsync<GroupMember> _groupMemberRepo;
    private readonly IGenericRepositoryAsync<UserEnrollment> _enrollmentRepo;
    private readonly IGenericRepositoryAsync<Experience> _expRepo;
    private readonly IGenericRepositoryAsync<Education> _eduRepo;
    private readonly IGenericRepositoryAsync<Project> _projRepo;
    private readonly IGenericRepositoryAsync<UserSkill> _skillRepo;
    private readonly IGenericRepositoryAsync<UserNodeProgress> _progressRepo;
    private readonly IGenericRepositoryAsync<RoadmapLevel> _levelRepo;
    private readonly IRoadmapService _roadmapService;

    public MemberService(
        UserManager<ApplicationUser> userManager,
        IGenericRepositoryAsync<UserProfile> profileRepo,
        IGenericRepositoryAsync<GroupMember> groupMemberRepo,
        IGenericRepositoryAsync<UserEnrollment> enrollmentRepo,
        IGenericRepositoryAsync<Experience> expRepo,
        IGenericRepositoryAsync<Education> eduRepo,
        IGenericRepositoryAsync<Project> projRepo,
        IGenericRepositoryAsync<UserSkill> skillRepo,
        IGenericRepositoryAsync<UserNodeProgress> progressRepo,
        IGenericRepositoryAsync<RoadmapLevel> levelRepo,
        IRoadmapService roadmapService)
    {
        _userManager = userManager;
        _profileRepo = profileRepo;
        _groupMemberRepo = groupMemberRepo;
        _enrollmentRepo = enrollmentRepo;
        _expRepo = expRepo;
        _eduRepo = eduRepo;
        _projRepo = projRepo;
        _skillRepo = skillRepo;
        _progressRepo = progressRepo;
        _levelRepo = levelRepo;
        _roadmapService = roadmapService;
    }

    public async Task<MemberDashboardDto?> GetMemberDashboardAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        var profile = await _profileRepo.GetTableNoTracking()
            .Include(p => p.Experiences)
            .Include(p => p.Educations)
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var groups = await _groupMemberRepo.GetTableNoTracking()
            .Include(gm => gm.Group)
                .ThenInclude(g => g.Roadmap)
            .Where(gm => gm.MemberId == userId)
            .Select(gm => new ActiveGroupDto
            {
                GroupId = gm.GroupId,
                Name = gm.Group.Name,
                RoadmapTitle = gm.Group.Roadmap != null ? gm.Group.Roadmap.Title : null
            })
            .ToListAsync();

        var enrollments = await _enrollmentRepo.GetTableNoTracking()
            .Include(e => e.Roadmap)
            .Where(e => e.UserId == userId)
            .Select(e => new ActiveRoadmapDto
            {
                RoadmapId = e.RoadmapId,
                Title = e.Roadmap.Title,
                ProgressPercentage = e.ProgressPercentage,
                Status = e.Status.ToString()
            })
            .ToListAsync();

        int completeness = 20; 
        if (profile != null)
        {
            if (!string.IsNullOrWhiteSpace(profile.ProfessionalSummary)) completeness += 20;
            if (profile.Experiences.Any()) completeness += 20;
            if (profile.Educations.Any()) completeness += 20;
            if (profile.Skills.Any()) completeness += 20;
        }

        return new MemberDashboardDto
        {
            UserId = userId,
            FullName = profile?.FullName ?? user.Email ?? "Member",
            ProfessionalSummary = profile?.ProfessionalSummary,
            ProfileCompletenessPercentage = completeness,
            ActiveGroups = groups,
            ActiveRoadmaps = enrollments
        };
    }

    public async Task<MemberProfileViewModel> GetProfileForEditAsync(Guid userId)
    {
        var profile = await _profileRepo.GetTableNoTracking()
            .Include(p => p.Experiences)
            .Include(p => p.Educations)
            .Include(p => p.Projects)
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var vm = new MemberProfileViewModel();
        if (profile == null) 
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            vm.FullName = user?.Email ?? "New User";
            vm.Email = user?.Email;
            return vm;
        }

        vm.FullName = profile.FullName;
        vm.Email = profile.ContactEmail;
        vm.Phone = profile.Phone;
        vm.Location = profile.Location;
        vm.ProfessionalSummary = profile.ProfessionalSummary;
        vm.GitHubUrl = profile.GitHubUrl;
        vm.LinkedInUrl = profile.LinkedInUrl;

        vm.Experiences = profile.Experiences.OrderByDescending(e => e.CreatedAt).Select(e => new ExperienceViewModel
        {
            Title = e.Title,
            Company = e.Company,
            Period = null, 
            Description = e.Description
        }).ToList();

        vm.Educations = profile.Educations.OrderByDescending(e => e.CreatedAt).Select(e => new EducationViewModel
        {
            Degree = e.Degree,
            University = e.University,
            Year = e.StartDate.Year.ToString()
        }).ToList();

        vm.Projects = profile.Projects.OrderByDescending(p => p.CreatedAt).Select(p => new ProjectViewModel
        {
            Name = p.Name,
            Description = p.Description,
            Period = p.Period,
            Url = p.Url
        }).ToList();

        vm.TechnicalSkills = profile.Skills.Where(s => s.Type == GDG_DashBoard.DAL.Eums.SkillType.Technical).Select(s => s.Name).ToList();
        vm.SoftSkills = profile.Skills.Where(s => s.Type == GDG_DashBoard.DAL.Eums.SkillType.SoftSkill).Select(s => s.Name).ToList();

        return vm;
    }

    public async Task UpdateMemberProfileAsync(Guid userId, MemberProfileViewModel model)
    {
        var profile = await _profileRepo.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile 
            { 
                UserId = userId,
                FullName = model.FullName
            };
            await _profileRepo.AddAsync(profile);
            await _profileRepo.SaveChangesAsync(); 
        }

        profile.FullName = model.FullName;
        profile.ContactEmail = model.Email;
        profile.Phone = model.Phone;
        profile.Location = model.Location;
        profile.ProfessionalSummary = model.ProfessionalSummary;
        profile.GitHubUrl = model.GitHubUrl;
        profile.LinkedInUrl = model.LinkedInUrl;

        await _profileRepo.SaveChangesAsync();

        var profileId = profile.Id;

        var exps = await _expRepo.GetTableNoTracking().Where(e => e.UserProfileId == profileId).ToListAsync();
        if (exps.Any()) await _expRepo.DeleteRangeAsync(exps);

        var edus = await _eduRepo.GetTableNoTracking().Where(e => e.UserProfileId == profileId).ToListAsync();
        if (edus.Any()) await _eduRepo.DeleteRangeAsync(edus);

        var projs = await _projRepo.GetTableNoTracking().Where(p => p.UserProfileId == profileId).ToListAsync();
        if (projs.Any()) await _projRepo.DeleteRangeAsync(projs);

        var skills = await _skillRepo.GetTableNoTracking().Where(s => s.UserProfileId == profileId).ToListAsync();
        if (skills.Any()) await _skillRepo.DeleteRangeAsync(skills);

        if (model.Experiences != null)
        {
            var newExps = model.Experiences
                .Where(e => !string.IsNullOrWhiteSpace(e.Title) && !string.IsNullOrWhiteSpace(e.Company))
                .Select(e => new Experience { UserProfileId = profileId, Title = e.Title, Company = e.Company, Description = e.Description, StartDate = DateTime.UtcNow })
                .ToList();
            if (newExps.Any()) await _expRepo.AddRangeAsync(newExps);
        }

        if (model.Educations != null)
        {
            var newEdus = model.Educations
                .Where(e => !string.IsNullOrWhiteSpace(e.Degree) && !string.IsNullOrWhiteSpace(e.University))
                .Select(e => new Education { UserProfileId = profileId, Degree = e.Degree, University = e.University, StartDate = DateTime.UtcNow })
                .ToList();
            if (newEdus.Any()) await _eduRepo.AddRangeAsync(newEdus);
        }

        if (model.Projects != null)
        {
            var newProjs = model.Projects
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .Select(p => new Project { UserProfileId = profileId, Name = p.Name, Description = p.Description, Period = p.Period, Url = p.Url })
                .ToList();
            if (newProjs.Any()) await _projRepo.AddRangeAsync(newProjs);
        }

        var newSkills = new List<UserSkill>();
        if (model.TechnicalSkills != null)
        {
            newSkills.AddRange(model.TechnicalSkills
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new UserSkill { UserProfileId = profileId, Name = s, Type = GDG_DashBoard.DAL.Eums.SkillType.Technical }));
        }
        if (model.SoftSkills != null)
        {
            newSkills.AddRange(model.SoftSkills
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => new UserSkill { UserProfileId = profileId, Name = s, Type = GDG_DashBoard.DAL.Eums.SkillType.SoftSkill }));
        }
        if (newSkills.Any()) await _skillRepo.AddRangeAsync(newSkills);

        await _profileRepo.SaveChangesAsync();
    }

    public async Task<RoadmapDetailsForMemberDto?> GetRoadmapDetailsForMemberAsync(Guid roadmapId, Guid userId)
    {
        var roadmap = await _roadmapService.GetRoadmapDetailsAsync(roadmapId);
        if (roadmap == null) return null;

        var levelIds = roadmap.Levels.Select(l => l.Id).ToList();
        var progresses = await _progressRepo.GetTableNoTracking()
            .Where(p => p.UserId == userId && levelIds.Contains(p.RoadmapLevelId))
            .ToListAsync();

        var isEnrolled = await _enrollmentRepo.GetTableNoTracking()
            .AnyAsync(e => e.UserId == userId && e.RoadmapId == roadmapId);

        return new RoadmapDetailsForMemberDto
        {
            Roadmap = roadmap,
            CompletedLevelIds = progresses.Where(p => p.IsCompleted).Select(p => p.RoadmapLevelId).ToHashSet(),
            TotalLevels = roadmap.Levels.Count,
            CompletedCount = progresses.Count(p => p.IsCompleted),
            IsEnrolled = isEnrolled
        };
    }

    public async Task<ToggleProgressResultDto> ToggleNodeProgressAsync(Guid userId, Guid levelId)
    {
        var progress = await _progressRepo.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.RoadmapLevelId == levelId);

        if (progress == null)
        {
            progress = new UserNodeProgress
            {
                UserId = userId,
                RoadmapLevelId = levelId,
                IsCompleted = true
            };
            await _progressRepo.AddAsync(progress);
        }
        else
        {
            progress.IsCompleted = !progress.IsCompleted;
        }

        await _progressRepo.SaveChangesAsync();

        var level = await _levelRepo.GetTableNoTracking()
            .FirstOrDefaultAsync(l => l.Id == levelId);

        if (level != null)
        {
            var roadmapLevelIds = await _levelRepo.GetTableNoTracking()
                .Where(l => l.RoadmapId == level.RoadmapId)
                .Select(l => l.Id)
                .ToListAsync();

            var completedCount = await _progressRepo.GetTableNoTracking()
                .CountAsync(p => p.UserId == userId && roadmapLevelIds.Contains(p.RoadmapLevelId) && p.IsCompleted);

            var totalLevels = roadmapLevelIds.Count;
            var percentage = totalLevels > 0 ? (int)Math.Round((double)completedCount / totalLevels * 100) : 0;

            var enrollment = await _enrollmentRepo.GetTableAsTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId && e.RoadmapId == level.RoadmapId);
            if (enrollment != null)
            {
                enrollment.ProgressPercentage = percentage;
                await _enrollmentRepo.SaveChangesAsync();
            }

            return new ToggleProgressResultDto
            {
                Success = true,
                IsCompleted = progress.IsCompleted,
                CompletedCount = completedCount,
                TotalLevels = totalLevels,
                Percentage = percentage
            };
        }

        return new ToggleProgressResultDto
        {
            Success = true,
            IsCompleted = progress.IsCompleted
        };
    }
}
