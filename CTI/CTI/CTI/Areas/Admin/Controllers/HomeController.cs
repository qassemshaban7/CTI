﻿using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace CTI.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
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
            var trainees = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainee
                                  select user).ToListAsync();

            var trainers = await (from user in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on user.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainer
                                  select user).ToListAsync();

            var courses = await _context.Courses.ToListAsync();

            var traineeUserIds = trainees.Select(u => u.Id).ToList();
            var traineeCourses = await _context.ApplicationUserCourses
                .Where(x => traineeUserIds.Contains(x.UserId))
                .ToListAsync();


            var surveys = await _context.Surveys.ToListAsync();

            SuperAdminHomeVM homeVM = new SuperAdminHomeVM
            {
                Trainee = trainees,
                Trainer = trainers,
                Courses = courses,
                TraineeCourse = traineeCourses,
                Surveys = surveys,
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
            if (oldPassword == null && newPassword == null)
            {
                {
                    x = 2;
                    return View("ChangePassword", new ChangePasswordViewModel { X = x });
                }
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
