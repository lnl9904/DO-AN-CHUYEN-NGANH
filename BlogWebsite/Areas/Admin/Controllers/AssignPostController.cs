using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.ViewModels.AssignPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;
using X.PagedList;

namespace BlogWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AssignPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISlugHelper _slugHelper;
        public AssignPostController(ApplicationDbContext context,
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

        [HttpGet("AssignArticles")]
        public async Task<IActionResult> Index(int? campaignId, string keyword, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            // Lấy danh sách Writing Phases để hiển thị dropdown
            var writingPhases = await _context.writingPhases!
                .OrderByDescending(wp => wp.StartDate)
                .Select(wp => new { wp.Id, wp.Title, wp.StartDate, wp.EndDate })
                .ToListAsync();

            ViewBag.WritingPhases = writingPhases;
            ViewBag.SelectedCampaign = campaignId;
            ViewBag.CurrentFilter = keyword;

            // Nếu không có đợt viết nào, trả về trang trống
            if (!writingPhases.Any())
            {
                return View(new PagedList<UserAssignmentViewModel>(new List<UserAssignmentViewModel>(), pageNumber, pageSize));
            }

            // Nếu không chọn campaignId, chọn mặc định đợt mới nhất
            if (!campaignId.HasValue)
            {
                campaignId = writingPhases.FirstOrDefault()?.Id;
                ViewBag.SelectedCampaign = campaignId;
            }

            // Truy vấn danh sách user có thể nhận bài viết
            var usersQuery = _context.userCapacities!
                .Where(uc => uc.WritingPhaseID == campaignId!.Value && uc.MaxAssignable > 0)
                .Select(uc => new UserAssignmentViewModel
                {
                    UserId = uc.ApplicationUserId,
                    FullName = uc.ApplicationUsers!.UserName,
                    MaxAssignable = uc.MaxAssignable
                });

            var countByCampaign = await _context.userCapacities!
            .Where(uc => uc.WritingPhaseID == campaignId!.Value)    
            .CountAsync();
            Console.WriteLine($"Số user thuộc WritingPhase {campaignId}: {countByCampaign}");

            // Áp dụng tìm kiếm nếu có keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                usersQuery = usersQuery.Where(u => u.FullName!.ToLower().Contains(keyword));
            }

            var pagedUsers = await usersQuery.ToPagedListAsync(pageNumber, pageSize);

            return View(pagedUsers);
        }


        [HttpPost]
        public async Task<IActionResult> Assign(string UserId, int CampaignId, int ArticleCount)
        {
            var userCapacity = await _context.userCapacities!
                .FirstOrDefaultAsync(uc => uc.ApplicationUserId == UserId && uc.WritingPhaseID == CampaignId);

            if (userCapacity == null || userCapacity.MaxAssignable < ArticleCount)
            {
                return BadRequest("Invalid assignment.");
            }

            // Thêm vào bảng Assignments
            var assignment = new Assignment
            {
                ApplicationUserId = UserId,
                WritingPhaseID = CampaignId,
                ArticleCount = ArticleCount
            };
            _context.assignments!.Add(assignment);

            // Cập nhật MaxAssignable
            userCapacity.MaxAssignable -= ArticleCount;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { campaignId = CampaignId });
        }

    }
}
