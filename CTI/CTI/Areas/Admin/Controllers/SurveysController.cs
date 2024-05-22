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

            var surveys = await _context.Surveys
                .Include(x => x.User)
                .Include(x => x.Course)
                .Include(x => x.Questions)
                .ThenInclude(y => y.Answers)
                .ToListAsync();

            return View(surveys);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainersByCourse(int courseId)
        {
            var trainers = await (from x in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on x.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  join auc in _context.ApplicationUserCourses
                                  on x.Id equals auc.UserId
                                  where role.Name == StaticDetails.Trainer && auc.CourseId == courseId
                                  select new { x.Id, x.UserFullName })
                                  .ToListAsync();

            return Json(trainers);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var model = new CreateSurveyVM
            {
                Questions = new List<QuestionVM>
                {
                    new QuestionVM
                    {
                        Answers = new List<AnswerVM>
                        {
                            new AnswerVM()
                        }
                    }
                }
            };

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
            ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
            ViewBag.Trainers = new SelectList(Enumerable.Empty<SelectListItem>());

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateSurveyVM model, string? returnUrl = null)
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
                    CourseId = model.CourseId,
                    UserId = model.UserId,
                    Questions = model.Questions.Select(q => new Question
                    {
                        QuestionName = q.QuestionName,
                        Answers = q.Answers.Select(a => new Answer
                        {
                            AnswerName = a.AnswerName,
                        }).ToList()
                    }).ToList()
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

                ViewBag.Courses = new SelectList(_context.Courses, "Id", "Name");
                ViewBag.Trainers = new SelectList(Enumerable.Empty<SelectListItem>());

                return View(model);
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
            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            var model = new EditSurveyVM
            {
                SurveyId = survey.Id,
                SurveyName = survey.SurveyName,
                StartDate = survey.StartDate,
                EndDate = survey.EndDate,
                ForWho = survey.ForWho,
                CourseId = survey.CourseId,
                UserId = survey.UserId,
                Questions = survey.Questions.Select(q => new EditQuestionVM
                {
                    QuestionId = q.Id,
                    QuestionName = q.QuestionName,
                    Answers = q.Answers.Select(a => new EditAnswerVM
                    {
                        AnswerId = a.Id,
                        AnswerName = a.AnswerName
                    }).ToList()
                }).ToList()
            };

            var trainers = await (from x in _context.ApplicationUsers
                                  join userRole in _context.UserRoles
                                  on x.Id equals userRole.UserId
                                  join role in _context.Roles
                                  on userRole.RoleId equals role.Id
                                  where role.Name == StaticDetails.Trainer
                                  select x)
                          .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.UserFullName })
                          .ToListAsync();

            var courses = await _context.Courses.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToListAsync();

            ViewBag.Trainers = trainers;
            ViewBag.Courses = courses;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditSurveyVM model)
        {
            try
            {
                var survey = await _context.Surveys
                    .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(s => s.Id == model.SurveyId);

                if (survey == null)
                {
                    return NotFound();
                }

                survey.SurveyName = model.SurveyName;
                survey.StartDate = model.StartDate;
                survey.EndDate = model.EndDate;
                survey.ForWho = model.ForWho;
                survey.CourseId = model.CourseId;
                survey.UserId = model.UserId;

                var existingQuestions = survey.Questions.ToList();

                foreach (var question in model.Questions)
                {
                    var existingQuestion = existingQuestions.FirstOrDefault(q => q.Id == question.QuestionId);
                    if (existingQuestion != null)
                    {
                        existingQuestion.QuestionName = question.QuestionName;

                        var existingAnswers = existingQuestion.Answers.ToList();

                        foreach (var answer in question.Answers)
                        {
                            var existingAnswer = existingAnswers.FirstOrDefault(a => a.Id == answer.AnswerId);
                            if (existingAnswer != null)
                            {
                                existingAnswer.AnswerName = answer.AnswerName;
                            }
                            else
                            {
                                existingQuestion.Answers.Add(new Answer { AnswerName = answer.AnswerName });
                            }
                        }

                        foreach (var existingAnswer in existingAnswers)
                        {
                            if (!question.Answers.Any(a => a.AnswerId == existingAnswer.Id))
                            {
                                _context.Answers.Remove(existingAnswer);
                            }
                        }
                    }
                    else
                    {
                        var newQuestion = new Question
                        {
                            QuestionName = question.QuestionName,
                            Answers = question.Answers.Select(a => new Answer { AnswerName = a.AnswerName }).ToList()
                        };

                        survey.Questions.Add(newQuestion);
                    }
                }

                foreach (var existingQuestion in existingQuestions)
                {
                    if (!model.Questions.Any(q => q.QuestionId == existingQuestion.Id))
                    {
                        _context.Questions.Remove(existingQuestion);
                    }
                }

                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("updated", "true");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var trainers = await (from x in _context.ApplicationUsers
                                      join userRole in _context.UserRoles
                                      on x.Id equals userRole.UserId
                                      join role in _context.Roles
                                      on userRole.RoleId equals role.Id
                                      where role.Name == StaticDetails.Trainer
                                      select x)
                                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.UserFullName })
                                .ToListAsync();

                var courses = await _context.Courses.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToListAsync();

                ViewBag.Trainers = trainers;
                ViewBag.Courses = courses;

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Results)
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
