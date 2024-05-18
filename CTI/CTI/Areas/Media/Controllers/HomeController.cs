using CTI.Data;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Data;
using System.Security.Claims;

namespace CTI.Areas.Media.Controllers
{
    [Authorize(Roles = StaticDetails.Media)]
    [Area(nameof(Media))]
    [Route(nameof(Media) + "/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.ApplicationUsers.FindAsync(userId);
            SuperAdminHomeVM homeVM = new SuperAdminHomeVM
            {
                MediaTeam = await (from x in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on x.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == StaticDetails.Media
                                   select x)
                                      .ToListAsync(),
            };
            return View(homeVM);
        }

        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(string oldPassword, string newPassword)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            int x = 0;
            if (newPassword == null)
            {
                x = 2;
                return View("ChangePassword", new ChangePasswordViewModel { X = x });
            }

            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!passwordVerificationResult)
            {
                x = 1;
                return View("ChangePassword", new ChangePasswordViewModel { X = x });
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            else
            {
                return View("ChangePassword", new ChangePasswordViewModel());
            }
        }
    }
}
