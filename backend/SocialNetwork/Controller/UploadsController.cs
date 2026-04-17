using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/uploads")]
[Authorize]
public class UploadsController : ApiControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public UploadsController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>Upload an image and return a public URL.</summary>
    [HttpPost("images")]
    [ProducesResponseType(typeof(ApiResponse<UploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequestResponse("File is required.");
        }

        if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequestResponse("Only image files are allowed.");
        }

        var fileName = Path.GetFileName(file.FileName);

        await using var stream = file.OpenReadStream();
        var url = await _fileStorageService.UploadAsync(
            stream,
            fileName,
            file.ContentType,
            HttpContext.RequestAborted);

        return OkResponse(new UploadResponse
        {
            Url = url,
            FileName = fileName
        });
    }
}
