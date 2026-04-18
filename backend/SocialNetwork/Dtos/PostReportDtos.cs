using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class PostReportCreateRequest
{
    [StringLength(128)]
    public string ReporterUserId { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Reason { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
}

public class PostReportReviewRequest
{
    public bool Reviewed { get; set; } = true;
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

public class PostReportDetailResponse
{
    public string PostReportId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string ReporterUserId { get; set; } = string.Empty;
    public string? ReporterUserName { get; set; }
    public string? ReporterFirstName { get; set; }
    public string? ReporterLastName { get; set; }
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PostAuthorUserId { get; set; } = string.Empty;
    public string? PostAuthorUserName { get; set; }
    public string? PostAuthorFirstName { get; set; }
    public string? PostAuthorLastName { get; set; }
    public string PostContent { get; set; } = string.Empty;
    public string? PostImageUrl { get; set; }
    public string? PostPrivacy { get; set; }
    public DateTime PostCreatedAt { get; set; }
}
