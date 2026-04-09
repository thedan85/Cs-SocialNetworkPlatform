using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class PostReportCreateRequest
{
    [Required]
    [StringLength(128)]
    public string ReporterUserId { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class PostReportResponse
{
    public string PostReportId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string ReporterUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
