using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/hashtags")]
[Authorize]
public class HashtagsController : ApiControllerBase
{
    private readonly IHashtagsService _hashtagsService;

    public HashtagsController(IHashtagsService hashtagsService)
    {
        _hashtagsService = hashtagsService;
    }

    /// <summary>Search hashtags by tag.</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<HashtagSearchResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchHashtags(
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int postsPerHashtag = 3)
    {
        var result = await _hashtagsService.SearchHashtagsAsync(
            query,
            pageNumber,
            pageSize,
            postsPerHashtag,
            HttpContext.RequestAborted);

        return FromServiceResult(result);
    }

    /// <summary>Get trending hashtags.</summary>
    [HttpGet("trending")]
    [ProducesResponseType(typeof(ApiResponse<List<HashtagTrendingResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrendingHashtags(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _hashtagsService.GetTrendingHashtagsAsync(
            pageNumber,
            pageSize,
            HttpContext.RequestAborted);

        return FromServiceResult(result);
    }
}
