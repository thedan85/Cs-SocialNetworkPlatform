using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkResponse<T>(T data)
    {
        return Ok(new ApiResponse<T> { Success = true, Data = data });
    }

    protected IActionResult CreatedResponse<T>(T data)
    {
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<T> { Success = true, Data = data });
    }

    protected IActionResult BadRequestResponse(string message)
    {
        return BadRequest(new ApiResponse<object> { Success = false, Errors = new List<string> { message } });
    }

    protected IActionResult BadRequestResponse(IEnumerable<string> errors)
    {
        return BadRequest(new ApiResponse<object> { Success = false, Errors = errors.ToList() });
    }

    protected IActionResult NotFoundResponse(string message)
    {
        return NotFound(new ApiResponse<object> { Success = false, Errors = new List<string> { message } });
    }

    protected IActionResult UnauthorizedResponse(string message)
    {
        return Unauthorized(new ApiResponse<object> { Success = false, Errors = new List<string> { message } });
    }

    protected IActionResult ConflictResponse(string message)
    {
        return Conflict(new ApiResponse<object> { Success = false, Errors = new List<string> { message } });
    }

    protected IActionResult ConflictResponse(IEnumerable<string> errors)
    {
        return Conflict(new ApiResponse<object> { Success = false, Errors = errors.ToList() });
    }

    protected IActionResult FromServiceResult<T>(ServiceResult<T> result, bool created = false)
    {
        if (result.Success)
        {
            return created ? CreatedResponse(result.Data) : OkResponse(result.Data);
        }

        return ErrorFromServiceResult(result.ErrorType, result.Errors);
    }

    private IActionResult ErrorFromServiceResult(ServiceErrorType? errorType, IReadOnlyList<string> errors)
    {
        var sanitizedErrors = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .ToList();

        if (sanitizedErrors.Count == 0)
        {
            sanitizedErrors.Add("The operation failed.");
        }

        return errorType switch
        {
            ServiceErrorType.NotFound => NotFoundResponse(sanitizedErrors[0]),
            ServiceErrorType.Conflict => ConflictResponse(sanitizedErrors),
            ServiceErrorType.Unauthorized => UnauthorizedResponse(sanitizedErrors[0]),
            _ => BadRequestResponse(sanitizedErrors)
        };
    }

    protected string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    protected bool IsCurrentUserOrAdmin(string userId)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return false;
        }

        return User.IsInRole("Admin")
            || string.Equals(currentUserId, userId, StringComparison.OrdinalIgnoreCase);
    }
}
