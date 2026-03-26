using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GDG_DashBoard.DAL.Models;
using GDG_DashBoard.BLL.Services.RoadmapServices;
using GDG_DashBoard.BLL.Services.Group;

namespace GDG_DashBoard.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IRoadmapService _roadmapService;
    private readonly IGroupService _groupService;

    public HomeController(IRoadmapService roadmapService, IGroupService groupService)
    {
        _roadmapService = roadmapService;
        _groupService = groupService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Roadmaps = await _roadmapService.GetPublicRoadmapsAsync();
        ViewBag.Groups = await _groupService.GetOpenCohortsAsync();
        return View();
    }
}
