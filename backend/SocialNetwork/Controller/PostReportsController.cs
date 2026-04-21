using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/post-reports")]
[Authorize(Roles = "Admin")]
public class PostReportsController : ApiControllerBase
{
    private readonly IPostsService _postsService;

    public PostReportsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    /// <summary>Get pending post reports.</summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<List<PostReportDetailResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingReports(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User context is missing.");
        }

        var result = await _postsService.GetPendingPostReportsAsync(
            currentUserId,
            isAdmin: true,
            pageNumber,
            pageSize,
            HttpContext.RequestAborted);

        return FromServiceResult(result);
    }

    /// <summary>Mark a post report as reviewed.</summary>
    [HttpPut("{postReportId}/review")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewReport(
        string postReportId,
        [FromBody] PostReportReviewRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User context is missing.");
        }

        var result = await _postsService.ReviewPostReportAsync(
            currentUserId,
            postReportId,
            request.Reviewed,
            isAdmin: true,
            HttpContext.RequestAborted);

        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        return OkResponse(new { message = result.Data });
    }
}
