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

                            var course = new Course
                            {
                                Name = worksheet.Cells[row, 1].Value.ToString(),
                                Duration = worksheet.Cells[row, 3].Value.ToString(),
                                Requirments = worksheet.Cells[row, 4].Value.ToString(),
                                Code = Convert.ToInt32(worksheet.Cells[row, 2].Value),
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

            var Course = await _context.Courses.ToListAsync();

            return View(Course);
        }
        public async Task<IActionResult> Create(string? returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                Course course = new()
                {
                    Name = model.Name,
                    Code = model.Code,
                    Requirments = model.Requirments,
                    Duration = model.Duration
                };

                _context.Courses.Add(course);

                try
                {
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("created", "true");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "خطا في بيانات المقرر");
                }
            }
            return View(model);
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
                Duration = course.Duration,
                Requirments = course.Requirments,
                Code = course.Code,
            };

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

            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    return NotFound();
                }

                course.Name = editUserVM.Name;
                course.Code = editUserVM.Code;
                course.Duration = editUserVM.Duration;
                course.Duration = editUserVM.Duration;

                _context.Update(course);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            return View(editUserVM);
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
                string FileName = "مثال اضافة المقررات.xlsx";
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
