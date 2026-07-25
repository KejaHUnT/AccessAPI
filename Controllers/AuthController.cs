using AccessAPI.Models;
using AccessAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if (identityUser is not null)
            {
                var checkPassword = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (checkPassword)
                {
                    var roles = await _userManager.GetRolesAsync(identityUser);

                    var jwtToken = _tokenRepository.CreateJwtToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto()
                    {
                        Email = request.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken
                    };

                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password Incorrect");

            return ValidationProblem(ModelState);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var user = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };
        
            var identityResult = await _userManager.CreateAsync(user, request.Password);
        
            if (identityResult.Succeeded)
            {
                var jwtToken = _tokenRepository.CreateJwtToken(user, new List<string>());
                return Ok(new LoginResponseDto
                {
                    Email = user.Email,
                    Roles = new List<string>(),
                    Token = jwtToken
                });
            }
        
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError("", error.Description);
        
            return ValidationProblem(ModelState);
        }
        
        [HttpPost("assign-role")]
        [Authorize]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AddUserToRoleRequestDto request)
        {
            if (request.RoleName != "Manager" && request.RoleName != "Tenant")
                return BadRequest("Role must be either 'Manager' or 'Tenant'.");
        
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();
        
            var roles = await _userManager.GetRolesAsync(user);
        
            if (!roles.Contains(request.RoleName))
            {
                var result = await _userManager.AddToRoleAsync(user, request.RoleName);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }
        
            var updatedRoles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenRepository.CreateJwtToken(user, updatedRoles.ToList());
        
            return Ok(new LoginResponseDto
            {
                Email = user.Email,
                Roles = updatedRoles.ToList(),
                Token = jwtToken
            });
        }
    }
}
