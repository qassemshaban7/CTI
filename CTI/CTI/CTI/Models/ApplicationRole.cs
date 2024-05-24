using Microsoft.AspNetCore.Identity;

namespace CTI.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string ArabicRoleName { get; set; }
    }
}
