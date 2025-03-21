using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly SignInManager<AppUser> signInManager;

    public AccountController(SignInManager<AppUser> signInManager)
    {
        this.signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var user = new AppUser
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Email = registerDTO.Email,
            UserName = registerDTO.Email
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }

    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();

        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        if (user == null) return Unauthorized();

        return Ok(
            new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Address = user.Address?.toDto()
            }
        );
    }

    [Authorize]
    [HttpPost("address")]

    public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDTO addressDTO){
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        if (user.Address == null){
            user.Address = addressDTO.toEntity();
        }
        else {
            user.Address.UpdateFromDto(addressDTO);
        }

        var result = await signInManager.UserManager.UpdateAsync(user);

        if (!result.Succeeded) return BadRequest("Problem when updating user address");

        return Ok(user.Address.toDto());
    }

    [HttpGet("auth-status")]
    public ActionResult GetAuthState()
    {
        return Ok(new { isAuthenticated = User.Identity?.IsAuthenticated ?? false });
    }
}