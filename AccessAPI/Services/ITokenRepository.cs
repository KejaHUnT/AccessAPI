using Microsoft.AspNetCore.Identity;

namespace AccessAPI.Services
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
        Task<string> GetUserDetailsFromToken(string token);
    }
}
