namespace AccessAPI.Models
{
    public class AddUserToRoleRequestDto
    {
        public string Email { get; set; } = default!;
        public string RoleName { get; set; } = default!;
    }
}
