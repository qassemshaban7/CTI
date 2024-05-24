using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CourseController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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
                    var uploads = Path.Combine(webRootPath, @"CoursesFiles\");
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

                            string  username = worksheet.Cells[row, 2].Value.ToString();
                            var user = await _context.ApplicationUsers.Include(u => u.Course).FirstOrDefaultAsync(x => x.UserName == username);
                            if (user.Course != null)
                            {
                                continue;
                            }

                            var course = new Course
                            {
                                Name = worksheet.Cells[row, 4].Value.ToString(),
                                Specialization = worksheet.Cells[row, 6].Value.ToString(),
                                Phase = worksheet.Cells[row, 7].Value.ToString(),
                                Department = worksheet.Cells[row, 5].Value.ToString(),
                                TypeDivition = worksheet.Cells[row, 8].Value.ToString(),
                                ApplicationUser = user,
                                UserId = user.Id,
                                Coursecode = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                                ReferenceNumber = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                            };

                            await _context.Courses.AddAsync(course);
                            await _context.SaveChangesAsync();
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

            var Course = await _context.Courses.Include(x => x.ApplicationUser).ToListAsync();

            return View(Course);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Trainer
                               select new ApplicationUser
                               {
                                   Id = x.Id,
                                   UserFullName = x.UserFullName
                               }).ToListAsync();

            ViewBag.Trainers = users;

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var user = await _context.ApplicationUsers.Include(u => u.Course).FirstOrDefaultAsync(u => u.Id == model.UserId);

                if (user == null)
                {
                    ModelState.AddModelError("", "المستخدم غير موجود");
                    var users = await (from x in _context.ApplicationUsers
                                       join userRole in _context.UserRoles
                                       on x.Id equals userRole.UserId
                                       join role in _context.Roles
                                       on userRole.RoleId equals role.Id
                                       where role.Name == StaticDetails.Trainer
                                       select new ApplicationUser
                                       {
                                           Id = x.Id,
                                           UserFullName = x.UserFullName
                                       }).ToListAsync();

                    ViewBag.Trainers = users;
                    return View(model);
                }

                if (user.Course != null)
                {
                    ModelState.AddModelError("", "هذا المستخدم لديه مقرر بالفعل");
                    var users = await (from x in _context.ApplicationUsers
                                       join userRole in _context.UserRoles
                                       on x.Id equals userRole.UserId
                                       join role in _context.Roles
                                       on userRole.RoleId equals role.Id
                                       where role.Name == StaticDetails.Trainer
                                       select new ApplicationUser
                                       {
                                           Id = x.Id,
                                           UserFullName = x.UserFullName
                                       }).ToListAsync();

                    ViewBag.Trainers = users;
                    return View(model);
                }
                Course course = new()
                {
                    Name = model.Name,
                    Coursecode = model.Coursecode,
                    Phase = model.Phase,
                    Specialization = model.Specialization,
                    TypeDivition = model.TypeDivition,
                    Department = model.Department,
                    ReferenceNumber = model.ReferenceNumber,
                    UserId = model.UserId,
                    ApplicationUser = user
                };

                _context.Courses.Add(course);

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "هذا المدرب لديه مقرر بالفaaعل");

                var users = await (from x in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on x.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == StaticDetails.Trainer
                                   select new ApplicationUser
                                   {
                                       Id = x.Id,
                                       UserFullName = x.UserFullName
                                   }).ToListAsync();

                ViewBag.Trainers = users;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var editUserVM = new EditCourseVM
            {
                Id = course.Id,
                Name = course.Name,
                Coursecode = course.Coursecode,
                Phase = course.Phase,
                Specialization = course.Specialization,
                TypeDivition = course.TypeDivition,
                Department = course.Department,
                ReferenceNumber = course.ReferenceNumber,
                UserId = course.UserId,

            };

            var users = await (from x in _context.ApplicationUsers
                               join userRole in _context.UserRoles
                               on x.Id equals userRole.UserId
                               join role in _context.Roles
                               on userRole.RoleId equals role.Id
                               where role.Name == StaticDetails.Trainer
                               select x)
                               .ToListAsync();

            ViewBag.Trainers = users;

            return View(editUserVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCourseVM editUserVM)
        {
            if (id != editUserVM.Id)
            {
                return NotFound();
            }

            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                var user = await _context.ApplicationUsers.Include(u => u.Course)
                                     .FirstOrDefaultAsync(u => u.Id == editUserVM.UserId);


                if (user.Course != null && user.Course.Id != id)
                {
                    ModelState.AddModelError("", "هذا المستخدم لديه مقرر بالفعل");

                    editUserVM = new EditCourseVM
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Coursecode = course.Coursecode,
                        Phase = course.Phase,
                        Specialization = course.Specialization,
                        TypeDivition = course.TypeDivition,
                        Department = course.Department,
                        ReferenceNumber = course.ReferenceNumber,
                        UserId = course.UserId
                    };

                    var users = await (from x in _context.ApplicationUsers
                                       join userRole in _context.UserRoles
                                       on x.Id equals userRole.UserId
                                       join role in _context.Roles
                                       on userRole.RoleId equals role.Id
                                       where role.Name == StaticDetails.Trainer
                                       select x)
                                       .ToListAsync();

                    ViewBag.Trainers = users;

                    return View(editUserVM);
                }

                course.Name = editUserVM.Name;
                course.Coursecode = editUserVM.Coursecode;
                course.Phase = editUserVM.Phase;
                course.Specialization = editUserVM.Specialization;
                course.TypeDivition = editUserVM.TypeDivition;
                course.Department = editUserVM.Department;
                course.ReferenceNumber = editUserVM.ReferenceNumber;
                course.UserId = editUserVM.UserId;
                course.ApplicationUser = user;

                _context.Update(course);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "خطا في بيانات المقرر.");
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                editUserVM = new EditCourseVM
                {
                    Id = course.Id,
                    Name = course.Name,
                    Coursecode = course.Coursecode,
                    Phase = course.Phase,
                    Specialization = course.Specialization,
                    TypeDivition = course.TypeDivition,
                    Department = course.Department,
                    ReferenceNumber = course.ReferenceNumber,
                    UserId = course.UserId
                };

                var users = await (from x in _context.ApplicationUsers
                                   join userRole in _context.UserRoles
                                   on x.Id equals userRole.UserId
                                   join role in _context.Roles
                                   on userRole.RoleId equals role.Id
                                   where role.Name == StaticDetails.Trainer
                                   select x)
                                   .ToListAsync();

                ViewBag.Trainers = users;

                return View(editUserVM);
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }


        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                string FileName = "المقررات.xlsx";
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

        private bool UserExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
