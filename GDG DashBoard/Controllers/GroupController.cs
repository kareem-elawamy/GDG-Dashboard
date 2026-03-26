using GDG_DashBoard.BLL.Services.Group;
using GDG_DashBoard.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GDG_DashBoard.Controllers;

[Authorize]
public class GroupController : Controller
{
    private readonly IGroupService _groupService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GroupController(IGroupService groupService, UserManager<ApplicationUser> userManager)
    {
        _groupService = groupService;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Organizer,Mentor")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer,Mentor")]
    public async Task<IActionResult> Create(string name, string description)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        await _groupService.CreateGroupAsync(name, description, user.Id);
        TempData["SuccessMessage"] = "Group created successfully!";
        return RedirectToAction("Index", "Instructor"); 
    }

    [Authorize(Roles = "Admin,Organizer,Mentor")]
    public async Task<IActionResult> Details(Guid id)
    {
        var group = await _groupService.GetGroupDetailsAsync(id);
        if (group == null) return NotFound();

        ViewBag.AvailableUsers = await _groupService.GetAvailableUsersAsync();
        ViewBag.AvailableRoadmaps = await _groupService.GetAvailableRoadmapsAsync();

        return View(group);
    }

    /// <summary>
    /// Read-only cohort view for standard Members. No management controls.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ViewCohort(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var group = await _groupService.GetGroupDetailsAsync(id);
        if (group == null) return NotFound();

        // Verify the member actually belongs to this group
        var isMember = group.GroupMembers?.Any(gm => gm.MemberId == user.Id) ?? false;
        var isManager = await _userManager.IsInRoleAsync(user, "Admin") 
                     || await _userManager.IsInRoleAsync(user, "Organizer") 
                     || await _userManager.IsInRoleAsync(user, "Mentor");

        if (!isMember && !isManager)
            return Forbid();

        return View(group);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer,Mentor")]
    public async Task<IActionResult> AddMembers(Guid groupId, List<Guid> memberIds)
    {
        if (memberIds != null && memberIds.Any())
        {
            await _groupService.AddMembersToGroupBulkAsync(groupId, memberIds);
            TempData["SuccessMessage"] = "Members added successfully!";
        }
        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Organizer,Mentor")]
    public async Task<IActionResult> AssignRoadmap(Guid groupId, Guid roadmapId)
    {
        await _groupService.AssignRoadmapToGroupAsync(groupId, roadmapId);
        TempData["SuccessMessage"] = "Roadmap assigned and enrollments triggered successfully!";
        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    [HttpGet]
    public async Task<IActionResult> Join(string code)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            // Check if user is already in this group
            var groups = await _groupService.GetAllGroupsAsync();
            var existingGroup = groups.FirstOrDefault(g => g.JoinCode == code);
            if (existingGroup != null && existingGroup.GroupMembers.Any(gm => gm.MemberId == user.Id))
            {
                return RedirectToAction("ViewCohort", new { id = existingGroup.Id });
            }
        }

        ViewBag.JoinCode = code;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> JoinPost(string code)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        try 
        {
            await _groupService.JoinGroupByCodeAsync(user.Id, code);
            TempData["SuccessMessage"] = "Successfully joined the group!";
            return RedirectToAction("MyProfile", "Member"); 
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Join", new { code });
        }
    }
}
