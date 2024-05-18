using CTI.Models;

namespace CTI.ViewModels
{
    public class SuperAdminHomeVM
    {
        public  IEnumerable<ApplicationUser>? Users { get; set; }
        public  IEnumerable<ApplicationUser>? PrepareTeam { get; set; }
        public  IEnumerable<ApplicationUser>? ReviewTeam { get; set; }
        public IEnumerable<ApplicationUser>? MediaTeam { get; set; }
    }
}
