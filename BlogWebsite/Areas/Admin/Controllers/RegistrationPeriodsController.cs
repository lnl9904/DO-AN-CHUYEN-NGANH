using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
using BlogWebsite.ViewModels.RegistrationPeriods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
using X.PagedList;

namespace BlogWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RegistrationPeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISlugHelper _slugHelper;
        public RegistrationPeriodsController(ApplicationDbContext context,
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

        [HttpGet("RegistrationPeriods")]
        public async Task<IActionResult> Index(string keyword, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 4;

            var loggedInUser = await _userManager.GetUserAsync(User);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            var registrationPeriodsQuery = _context.registrationPeriods!
                .Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUsers!.Id == loggedInUser!.Id)
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new RPVM
                {
                    Id = x.Id,
                    Title = x.Title,
                    RegisStart = x.RegisStart,
                    RegisEnd = x.RegisEnd,
                    RegisDeadlineStart = x.RegisDeadlineStart,
                    RegisDeadlineEnd = x.RegisDeadlineEnd,
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

        [HttpGet("CreateRPs")]
        public IActionResult CreateRPs()
        {
            return View(new CreateRegistrationPeriodsVM());
        }

        [HttpPost("CreateRPs")]
        public async Task<IActionResult> CreateRPs(CreateRegistrationPeriodsVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);

            var rps = new RegistrationPeriods
            {
                Title = vm.Title,
                RegisStart = vm.RegisStart,
                RegisEnd = vm.RegisEnd,
                RegisDeadlineStart = vm.RegisDeadlineStart,
                RegisDeadlineEnd = vm.RegisDeadlineEnd,
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

            _context.registrationPeriods!.Add(rps);
            await _context.SaveChangesAsync();
            _notification.Success("Writing Phase Created Successfully!");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRegistrationPeriods(int id)
        {
            var rps = await _context.registrationPeriods!.SingleOrDefaultAsync(x => x.Id == id);

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebsiteRole.WebisteAdmin || loggedInUser?.Id == rps?.ApplicationUserId)
            {
                _context.registrationPeriods!.Remove(rps!);
                await _context.SaveChangesAsync();
                _notification.Success("Period regis Deleted Successfully!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            else
            {
                _notification.Error("You are not Authorized!");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
        }

        [HttpGet("EditRPs")]
        public async Task<IActionResult> EditRPs(int id)
        {
            var rps = await _context.registrationPeriods!
                .SingleOrDefaultAsync(x => x.Id == id);

            if (rps == null)
            {
                _notification.Error("Regis Period not found!");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != WebsiteRole.WebisteAdmin && loggedInUser!.Id != rps.ApplicationUserId)
            {
                _notification.Error("You Are Not Author Of This RP!");
                return RedirectToAction("Index");
            }

            var vm = new EditRegistrationPeriodsVM()
            {
                Id = rps.Id,
                Title = rps.Title,
                RegisStart = rps.RegisStart,
                RegisEnd = rps.RegisEnd,
                ModifiedAt = rps.ModifiedAt,
                RegisDeadlineStart = rps.RegisDeadlineStart,
                RegisDeadlineEnd = rps.RegisDeadlineEnd,
                Is_Opening_registration = rps.Is_Opening_registration,
            };

            return View(vm);
        }

        [HttpPost("EditRPs")]
        public async Task<IActionResult> EditRPs(EditRegistrationPeriodsVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var rps = await _context.registrationPeriods!.SingleOrDefaultAsync(x => x.Id == vm.Id);
            if (rps == null)
            {
                _notification.Error("Regis Period not found!");
                return View();
            }

            rps.Title = vm.Title;
            rps.RegisStart = vm.RegisStart;
            rps.RegisEnd = vm.RegisEnd;
            rps.RegisDeadlineStart = vm.RegisDeadlineStart;
            rps.RegisDeadlineEnd = vm.RegisDeadlineEnd;
            rps.ModifiedAt = vm.ModifiedAt;
            rps.Is_Opening_registration = vm.Is_Opening_registration;

            await _context.SaveChangesAsync();
            _notification.Success("Regis Period Updated Successfully!");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
    }
}
