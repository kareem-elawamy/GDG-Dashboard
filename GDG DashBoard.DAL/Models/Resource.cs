using GDG_DashBoard.DAL.Eums;

namespace GDG_DashBoard.DAL.Models;

public class Resource : BaseEntity
{
    public Guid RoadmapLevelId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Title { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public required string Url { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(250)]
    public string? ThumbnailUrl { get; set; }
    public int EstimatedMinutes { get; set; }
    public int OrderIndex { get; set; }
    public ResourceType Type { get; set; }

    public RoadmapLevel RoadmapLevel { get; set; } = null!;
}