using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Data;
using System.Security.Claims;

namespace CTI.Areas.Trainee.Controllers
{
    [Authorize(Roles = StaticDetails.Trainee)]
    [Area(nameof(Trainee))]
    [Route(nameof(Trainee) + "/[controller]/[action]")]
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

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers
                                     .Include(u => u.ApplicationUserCourses)
                                     .ThenInclude(uc => uc.Course)
                                     .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var registeredCourses = await _context.ApplicationUserCourses
                                                  .Where(uc => uc.UserId == userId)
                                                  .Select(uc => new Course
                                                  {
                                                      Name = uc.Course.Name,
                                                      Phase = uc.Course.Phase,
                                                      Specialization = uc.Course.Specialization,
                                                      ReferenceNumber = uc.Course.ReferenceNumber,
                                                      Coursecode = uc.Course.Coursecode,
                                                      Department = uc.Course.Department,
                                                      TypeDivition = uc.Course.TypeDivition
                                                  }).ToListAsync();

            var viewModel = new ApplicationUser
            {
                UserFullName = user.UserFullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                TrainingProgram = user.TrainingProgram,
                Email = user.Email,
                EnglishFullName = user.EnglishFullName,
                RegisterNum = user.RegisterNum,
            };

            ViewBag.registeredCourses = registeredCourses;
            return View(viewModel);
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
