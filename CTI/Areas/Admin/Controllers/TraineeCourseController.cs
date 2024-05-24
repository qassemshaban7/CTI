using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace CTI.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class TraineeCourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TraineeCourseController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
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
                    var uploads = Path.Combine(webRootPath, @"TraineeCoursesFiles\");
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

                            long courseCode = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                            var course = await _context.Courses.FirstOrDefaultAsync(x => x.ReferenceNumber == courseCode);

                            string trainingNumber = worksheet.Cells[row, 1].Value.ToString();
                            var Trainee = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == trainingNumber);


                            var selectedTraineeCourses = await _context.ApplicationUserCourses
                                      .Where(ptu => ptu.CourseId == course.Id && ptu.UserId == Trainee.Id)
                                      .FirstOrDefaultAsync();

                            if(selectedTraineeCourses != null)
                            {
                                continue;
                            }

                            var Traineecourse = new ApplicationUserCourse
                            {
                                UserId = Trainee.Id,
                                CourseId = course.Id
                            };

                            await _context.ApplicationUserCourses.AddAsync(Traineecourse);
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

        public async Task<IActionResult> Create(int Id, string? returnUrl = null)
        {
            if (Id == null)
            {
                return NotFound();
            }
            
            var course = await _context.Courses.FindAsync(Id);
            if (course == null)
            {
                return NotFound();
            }

            var selectedTraineeCourses = await _context.ApplicationUserCourses
              .Where(ptu => ptu.CourseId == Id)
              .Select(ptu => ptu.UserId)
              .ToListAsync();

            var Trainees = await (from x in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on x.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainee
                                  select x)
                            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.UserFullName })
                            .ToListAsync();

            ViewBag.Trainees = Trainees;
            ViewBag.selectedTraineeCourses = selectedTraineeCourses;

            var traineecourse = new CreateTraineeCourseVM
            {
                UserIds = selectedTraineeCourses,
                CourseId = course.Id,
                Name = course.Name,
            };

            return View(traineecourse);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CreateTraineeCourseVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            try
            {
                var course = await _context.Courses.FindAsync(model.CourseId);
                var existingEntries = await _context.ApplicationUserCourses
                          .Where(ptu => ptu.CourseId == model.CourseId)
                          .ToListAsync();

                foreach (var entry in existingEntries)
                {
                    _context.ApplicationUserCourses.Remove(entry);
                }
                await _context.SaveChangesAsync();

                if (model.UserIds == null)
                {
                    ModelState.AddModelError("", "يجب اختيار متدربين");
                    var selectedTraineeCourses = await _context.ApplicationUserCourses
                          .Where(ptu => ptu.CourseId == model.CourseId)
                          .Select(ptu => ptu.UserId)
                          .ToListAsync();

                    var Trainees = await (from x in _context.ApplicationUsers
                                          join userRole in _context.UserRoles
                                          on x.Id equals userRole.UserId
                                          join role in _context.Roles
                                          on userRole.RoleId equals role.Id
                                          where role.Name == StaticDetails.Trainee
                                          select x)
                                    .ToListAsync();

                    ViewBag.Trainees = Trainees;
                    ViewBag.selectedTraineeCourses = selectedTraineeCourses;

                    var traineecourse = new CreateTraineeCourseVM
                    {
                        UserIds = selectedTraineeCourses,
                        CourseId = course.Id,
                        Name = course.Name,
                    };

                    return View(traineecourse);

                }

                foreach (var userId in model.UserIds)
                {
                    var selectedTraineeCourses = await _context.ApplicationUserCourses
                                      .Where(ptu => ptu.CourseId == model.CourseId && ptu.UserId == userId)
                                      .FirstOrDefaultAsync();

                    if (selectedTraineeCourses != null)
                    {
                        continue;
                    }

                    var traineecourse = new ApplicationUserCourse
                    {
                        UserId = userId,
                        CourseId = model.CourseId
                    };
                    _context.ApplicationUserCourses.Add(traineecourse);
                }

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");
            }
            catch
            {
                var selectedTraineeCourses = await _context.ApplicationUserCourses
                          .Where(ptu => ptu.CourseId == model.CourseId)
                          .Select(ptu => ptu.UserId)
                          .ToListAsync();

                var Trainees = await (from x in _context.ApplicationUsers
                                      join userRole in _context.UserRoles
                                      on x.Id equals userRole.UserId
                                      join role in _context.Roles
                                      on userRole.RoleId equals role.Id
                                      where role.Name == StaticDetails.Trainee
                                      select x)
                                .ToListAsync();

                ViewBag.Trainees = Trainees;
                ViewBag.selectedTraineeCourses = selectedTraineeCourses;

                var course = await _context.Courses.FindAsync(model.CourseId);
                
                var traineecourse = new CreateTraineeCourseVM
                {
                    UserIds = selectedTraineeCourses,
                    CourseId = course.Id,
                    Name = course.Name,
                };

                return View(traineecourse);
            }
        }

       
        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                string FileName = "مثال اضافة المقررات للمتدربين.xlsx";
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
