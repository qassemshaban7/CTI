using CTI.Data;
using CTI.Models;
using CTI.Utility;
using CTI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CTI.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Admin)]
    [Area(nameof(Admin))]
    [Route(nameof(Admin) + "/[controller]/[action]")]
    public class SurveysController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SurveysController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment
            , UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
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

            var surveys = await _context.Surveys.ToListAsync();

            return View(surveys);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetTrainersByCourse(int courseId)
        //{
        //    var trainers = await (from x in _context.ApplicationUsers
        //                          join userRole in _context.UserRoles
        //                          on x.Id equals userRole.UserId
        //                          join role in _context.Roles
        //                          on userRole.RoleId equals role.Id
        //                          join auc in _context.ApplicationUserCourses
        //                          on x.Id equals auc.UserId
        //                          where role.Name == StaticDetails.Trainer && auc.CourseId == courseId
        //                          select new { x.Id, x.UserFullName })
        //                          .ToListAsync();

        //    return Json(trainers);
        //}

        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            //var model = new CreateSurveyVM
            //{
            //    Questions = new List<QuestionVM>
            //    {
            //        new QuestionVM
            //        {
            //            Answers = new List<AnswerVM>
            //            {
            //                new AnswerVM()
            //            }
            //        }
            //    }
            //};

            //var trainers = await (from x in _context.ApplicationUsers
            //                      join userRole in _context.UserRoles
            //                      on x.Id equals userRole.UserId
            //                      join role in _context.Roles
            //                      on userRole.RoleId equals role.Id
            //                      where role.Name == StaticDetails.Trainer
            //                      select x)
            //              .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.UserFullName })
            //              .ToListAsync();

            //var Courses = await _context.Courses.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToListAsync();

            //ViewBag.Trainers = trainers;

            //ViewBag.Courses = Courses;
            //ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            //ViewBag.Trainers = new SelectList(Enumerable.Empty<SelectListItem>());

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Survey model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var survey = new Survey
                {
                    SurveyName = model.SurveyName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ForWho = model.ForWho,
                    SurveyType = model.SurveyType,
                    //Questions = model.Questions.Select(q => new Question
                    //{
                    //    QuestionName = q.QuestionName,
                    //    Answers = q.Answers.Select(a => new Answer
                    //    {
                    //        AnswerName = a.AnswerName,
                    //    }).ToList()
                    //}).ToList()
                };

                _context.Surveys.Add(survey);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("created", "true");
                return RedirectToAction("Index");
            }
            catch
            {
                //var trainers = await (from x in _context.ApplicationUsers
                //                      join userRole in _context.UserRoles
                //                      on x.Id equals userRole.UserId
                //                      join role in _context.Roles
                //                      on userRole.RoleId equals role.Id
                //                      where role.Name == StaticDetails.Trainer
                //                      select x)
                //           .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.UserFullName })
                //           .ToListAsync();

                //var Courses = await _context.Courses.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToListAsync();

                //ViewBag.Trainers = trainers;

                //ViewBag.Courses = Courses;

                

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainersByCourse2(int courseId)
        {
            var trainers = await (from x in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on x.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  join courseTrainer in _context.ApplicationUserCourses
                                  on x.Id equals courseTrainer.UserId
                                  where role.Name == StaticDetails.Trainer && courseTrainer.CourseId == courseId
                                  select new SelectListItem
                                  {
                                      Value = x.Id.ToString(),
                                      Text = x.UserFullName
                                  }).ToListAsync();

            return Json(trainers);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Survey model)
        {
            try
            {
                var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == model.Id);

                if (survey == null)
                {
                    return NotFound();
                }

                survey.SurveyName = model.SurveyName;
                survey.StartDate = model.StartDate;
                survey.EndDate = model.EndDate;
                survey.ForWho = model.ForWho;
                survey.SurveyType = model.SurveyType;

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("deleted", "true");
            return RedirectToAction(nameof(Index));
        }
    }
}
