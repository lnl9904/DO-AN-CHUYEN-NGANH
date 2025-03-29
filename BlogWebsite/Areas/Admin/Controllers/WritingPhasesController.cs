using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels.RegistrationPeriods;
using BlogWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Slugify;
using BlogWebsite.ViewModels.WritingPhases;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WritingPhasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISlugHelper _slugHelper;
        public WritingPhasesController(ApplicationDbContext context,
                              INotyfService notyfService,
                              IWebHostEnvironment webHostEnvironment,
                              UserManager<ApplicationUser> userManager,
                              ISlugHelper slugHelper)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _notification = notyfService;
            _slugHelper = slugHelper;
        }

        [HttpGet("WritingPhases")]
        public async Task<IActionResult> Index(string keyword, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 4;

            var loggedInUser = await _userManager.GetUserAsync(User);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            var registrationPeriodsQuery = _context.writingPhases!
                .Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUsers!.Id == loggedInUser!.Id)
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new WPVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    RegistrationPeriodID = x.RegistrationPeriodID,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    AmountArticles = x.AmountArticles,
                    CreateAt = x.CreateAt,
                    ModifiedAt = x.ModifiedAt,
                    AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author",
                    Is_Opening_registration = x.Is_Opening_registration,
                });

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                registrationPeriodsQuery = registrationPeriodsQuery.Where(x => x.Title!.ToLower().Contains(keyword));
            }

            var listPost_Page = await registrationPeriodsQuery.ToPagedListAsync(pageNumber, pageSize);

            return View(listPost_Page);
        }

        [HttpGet("CreateWPs")]
        public IActionResult CreateWPs()
        {
            var model = new CreateWritingPhasesVM
            {
                RegistrationPeriods = _context.registrationPeriods!
                                      .Select(r => new SelectListItem
                                      {
                                          Value = r.Id.ToString(),
                                          Text = r.Title
                                      })
                                      .ToList()
            };

            return View(model);
        }

        [HttpPost("CreateWPs")]
        public async Task<IActionResult> CreateWPs(CreateWritingPhasesVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Load lại dropdown nếu có lỗi
                vm.RegistrationPeriods = _context.registrationPeriods!
                                                 .Select(r => new SelectListItem
                                                 {
                                                     Value = r.Id.ToString(),
                                                     Text = r.Title
                                                 })
                                                 .ToList();
                return View(vm);
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var rps = new WritingPhases
            {
                Title = vm.Title,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                RegistrationPeriodID = vm.RegistrationPeriodID,
                AmountArticles = vm.AmountArticles,
                ModifiedAt = vm.ModifiedAt,
                Is_Opening_registration = vm.Is_Opening_registration,
                ApplicationUserId = loggedInUser!.Id
            };

            if (rps.Title != null)
            {
                var slugHelper = new SlugHelper();
                string slug = slugHelper.GenerateSlug(vm.Title!.Trim());

                rps.Slug = slug + "-" + Guid.NewGuid();
            }

            _context.writingPhases!.Add(rps);
            await _context.SaveChangesAsync();
            _notification.Success("Writing Phase Created Successfully!");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWritingPhases(int id)
        {
            var rps = await _context.writingPhases!.SingleOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebsiteRole.WebisteAdmin || loggedInUser?.Id == rps?.ApplicationUserId)
            {
                _context.writingPhases!.Remove(rps!);
                await _context.SaveChangesAsync();
                _notification.Success("Writing Phase Deleted Successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            else
            {
                _notification.Error("You are not Authorized!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
        }

        [HttpGet("EditWPs")]
        public async Task<IActionResult> EditWPs(int id)
        {
            var rps = await _context.writingPhases!
                .Include(x => x.RegistrationPeriods) // Load thêm RegistrationPeriod nếu cần
                .SingleOrDefaultAsync(x => x.Id == id);

            if (rps == null)
            {
                _notification.Error("Writing Phase not found!");
                return RedirectToAction("Index");
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng nếu chưa đăng nhập
            }

            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser);
            if (!loggedInUserRole.Contains(WebsiteRole.WebisteAdmin) && loggedInUser.Id != rps.ApplicationUserId)
            {
                return Forbid(); // Trả về 403 nếu không có quyền
            }

            var vm = new EditWritingPhasesVM()
            {
                Id = rps.Id,
                Title = rps.Title,
                StartDate = rps.StartDate,
                EndDate = rps.EndDate,
                AmountArticles = rps.AmountArticles,
                RegistrationPeriodID = rps.RegistrationPeriodID,
                Is_Opening_registration = rps.Is_Opening_registration,
                ModifiedAt = rps.ModifiedAt,
                RegistrationPeriods = _context.registrationPeriods!
                                              .Select(r => new SelectListItem
                                              {
                                                  Value = r.Id.ToString(),
                                                  Text = r.Title
                                              })
                                              .ToList()
            };

            return View(vm);
        }

        [HttpPost("EditWPs")]
        public async Task<IActionResult> EditWPs(EditWritingPhasesVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Load lại dropdown nếu có lỗi
                vm.RegistrationPeriods = _context.registrationPeriods!
                                                 .Select(r => new SelectListItem
                                                 {
                                                     Value = r.Id.ToString(),
                                                     Text = r.Title
                                                 })
                                                 .ToList();
                return View(vm);
            }

            var rps = await _context.writingPhases!.SingleOrDefaultAsync(x => x.Id == vm.Id);
            if (rps == null)
            {
                _notification.Error("Writing Phase not found!");
                return RedirectToAction("Index");
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            if (loggedInUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser);
            if (!loggedInUserRole.Contains(WebsiteRole.WebisteAdmin) && loggedInUser.Id != rps.ApplicationUserId)
            {
                return Forbid();
            }

            // Cập nhật dữ liệu
            rps.Title = vm.Title;
            rps.StartDate = vm.StartDate;
            rps.EndDate = vm.EndDate;
            rps.AmountArticles = vm.AmountArticles;
            rps.RegistrationPeriodID = vm.RegistrationPeriodID;
            rps.ModifiedAt = vm.ModifiedAt; // Set mặc định nếu null
            rps.Is_Opening_registration = vm.Is_Opening_registration;

            await _context.SaveChangesAsync();
            _notification.Success("Writing Phase Updated Successfully!");
            return RedirectToAction("Index", "WritingPhases", new { area = "Admin" });
        }


        //[HttpGet("EditWPs")]
        //public async Task<IActionResult> EditWPs(int id)
        //{
        //    var rps = await _context.registrationPeriods!
        //        .SingleOrDefaultAsync(x => x.Id == id);

        //    if (rps == null)
        //    {
        //        _notification.Error("Writing Phase not found!");
        //        return View();
        //    }

        //    var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
        //    var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
        //    if (loggedInUserRole[0] != WebsiteRole.WebisteAdmin && loggedInUser!.Id != rps.ApplicationUserId)
        //    {
        //        _notification.Error("You Are Not Author Of This Writing Phase!");
        //        return RedirectToAction("Index");
        //    }

        //    var vm = new EditRegistrationPeriodsVM()
        //    {
        //        Id = rps.Id,
        //        Title = rps.Title,
        //        RegisStart = rps.RegisStart,
        //        RegisEnd = rps.RegisEnd,
        //        RegisDeadlineStart = rps.RegisDeadlineStart,
        //        RegisDeadlineEnd = rps.RegisDeadlineEnd,
        //        Is_Opening_registration = rps.Is_Opening_registration,
        //        ModifiedAt = rps.ModifiedAt,
        //    };

        //    return View(vm);
        //}

        //[HttpPost("EditWPs")]
        //public async Task<IActionResult> EditWPs(EditWritingPhasesVM vm)
        //{
        //    if (!ModelState.IsValid) { return View(vm); }
        //    var rps = await _context.writingPhases!.SingleOrDefaultAsync(x => x.Id == vm.Id);
        //    if (rps == null)
        //    {
        //        _notification.Error("Writing Phase not found!");
        //        return View();
        //    }

        //    rps.Title = vm.Title;
        //    rps.StartDate = vm.StartDate;
        //    rps.EndDate = vm.EndDate;
        //    rps.AmountArticles = vm.AmountArticles;
        //    rps.RegistrationPeriodID = vm.RegistrationPeriodID;
        //    rps.ModifiedAt = vm.ModifiedAt;
        //    rps.Is_Opening_registration = vm.Is_Opening_registration;

        //    await _context.SaveChangesAsync();
        //    _notification.Success("Writing Phase Updated Successfully!");
        //    return RedirectToAction("Index", "WritingPhases", new { area = "Admin" });

        //}
    }
}
