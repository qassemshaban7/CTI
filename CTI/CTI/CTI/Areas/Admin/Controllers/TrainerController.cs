using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace CTI.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TrainerController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add()
        {
            try
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var uploads = Path.Combine(webRootPath, @"TrainerFiles\");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + files[0].FileName;

                    using (var filesStream = new FileStream(Path.Combine(uploads, uniqueFileName), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    using (var package = new ExcelPackage(new FileInfo(Path.Combine(uploads, uniqueFileName))))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {

                            var user = new ApplicationUser
                            {
                                UserFullName = worksheet.Cells[row, 1].Value.ToString(),
                                UserName = worksheet.Cells[row, 2].Value.ToString(),
                                Degree = worksheet.Cells[row, 3].Value.ToString(),
                                EmailConfirmed = true,
                            };

                            var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    HttpContext.Session.SetString("created", "true");
                }
                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> Index()
        {

            if (HttpContext.Session.GetString("created") != null)
            {
                ViewBag.created = true;
                HttpContext.Session.Remove("created");
            }
            if (HttpContext.Session.GetString("updated") != null)
            {
                ViewBag.updated = true;
                HttpContext.Session.Remove("updated");
            }
            if (HttpContext.Session.GetString("deleted") != null)
            {
                ViewBag.deleted = true;
                HttpContext.Session.Remove("deleted");
            }

            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Trainer
                               select x)
                               .ToListAsync();

            return View(users);
        }
        public async Task<IActionResult> Create(string? returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainerVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserFullName = model.UserFullName,
                    UserName = model.UserName,
                    Degree = model.Degree,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, StaticDetails.Trainer);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("created", "true");
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var editUserVM = new EditTrainerVM
            {
                Id = user.Id,
                UserFullName = user.UserFullName,
                UserName = user.UserName,
                Degree = user.Degree,

            };

            return View(editUserVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditTrainerVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.ApplicationUsers.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserFullName = editUserVM.UserFullName;
                user.UserName = editUserVM.UserName;
                user.Degree = editUserVM.Degree;

                _context.Update(user);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            return View(editUserVM);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);
            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }
        

        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                string FileName = "مثال اضافة المدربين.xlsx";
                string exampleFolder = Path.Combine(_hostingEnvironment.WebRootPath, "example");
                var filePath = Path.Combine(exampleFolder, FileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(fileBytes, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}