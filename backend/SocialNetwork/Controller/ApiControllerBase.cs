using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;

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
}
