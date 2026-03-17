using Microsoft.AspNetCore.Identity;

namespace CivicRequest.API.Models
{
    public class Officer : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Officer";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}