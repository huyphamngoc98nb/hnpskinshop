// This controller is used for testing various HTTP response scenarios in the SkiShop project.
// It provides endpoints to simulate different types of HTTP responses such as Unauthorized, BadRequest, NotFound, 
// InternalServerError, and ValidationError. These endpoints are useful for testing and debugging purposes.
using System.Security.Claims;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiShop.API.DTOs;

namespace API.Controllers;


public class BugController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized();
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("This was a bad request");
    }

     [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound("This was not found");
    }

    [HttpGet("internalerror")]
    public IActionResult GetInteralError()
    {
        throw new Exception("This was test exception");
    }

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDTO product)
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(name + " " + id);
    }
}