using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebsite.Data;
using BlogWebsite.Models;
using BlogWebsite.Utilites;
using BlogWebsite.ViewModels;
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
    [Route("Admin/Post")]
    public class PostController : Controller
	{
		private readonly ApplicationDbContext _context;
		public INotyfService _notification { get; }
		private IWebHostEnvironment _webHostEnvironment;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ISlugHelper _slugHelper;
		public PostController(ApplicationDbContext context,
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

		[HttpGet("Post")]
		public async Task<IActionResult> Index(string? keyword, bool? isPost, int? page)
		{
			int pageNumber = page ?? 1;
			int pageSize = 4;

			var loggedInUser = await _userManager.GetUserAsync(User);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

			var postsQuery = _context.posts!
				.Where(x => loggedInUserRole[0] == WebsiteRole.WebisteAdmin || x.ApplicationUsers!.Id == loggedInUser!.Id)
				.OrderByDescending(x => x.CreatedDate)
				.Select(x => new PostVM
				{
					Id = x.Id,
					Title = x.Title,
					TagName = x.Tag != null ? x.Tag.Name : "None Tag",
					ViewCount = x.ViewCount,
					CreateDate = x.CreatedDate,
					ModifiedDate = x.ModifiedDate,
					ThumbnailUrl = x.ThumbnailUrl,
					WritingPhases = x.WritingPhases != null ? x.WritingPhases.Title : "N/A",
					AuthorName = x.ApplicationUsers != null ? x.ApplicationUsers.FirstName + " " + x.ApplicationUsers.LastName : "Unknown Author",
					LikeCount = x.LikeCount,
					IsPost = x.IsPost // ✅ Đổi từ IsPublished sang IsPost
				});

			// ✅ Lọc theo trạng thái IsPost
			if (isPost.HasValue)
			{
				postsQuery = postsQuery.Where(x => x.IsPost == isPost.Value);
			}

			// ✅ Lọc theo từ khóa tìm kiếm
			if (!string.IsNullOrEmpty(keyword))
			{
				keyword = keyword.ToLower();
				postsQuery = postsQuery.Where(x => x.Title!.ToLower().Contains(keyword));
			}

			var listPost_Page = await postsQuery.ToPagedListAsync(pageNumber, pageSize);

			ViewBag.CurrentFilter = keyword;
			ViewBag.CurrentStatus = isPost; // ✅ Lưu trạng thái để hiển thị lại trên UI

			return View(listPost_Page);
		}



		[HttpGet("CreatePost")]
        public async Task<IActionResult> CreatePost()
        {
            var model = new CreatPostVM
            {
                WritingPhases = await _context.writingPhases!.ToListAsync()
            };
            return View(model);
        }
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost(CreatPostVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Lấy danh sách WritingPhases từ database để hiển thị lại form nếu có lỗi
                vm.WritingPhases = await _context.writingPhases!.ToListAsync();
                return View(vm);
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var tag = await _context.tags!.FirstOrDefaultAsync(t => t.Name == vm.TagName);

            if (tag == null)
            {
                tag = new Tag
                {
                    Name = vm.TagName!.ToUpper()
                };

                _context.tags!.Add(tag);
                await _context.SaveChangesAsync();
            }

            var post = new Post
            {
                Title = vm.Title,
                WritingPhaseID = vm.WritingPhaseID,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Description = vm.Description,
                TagId = tag!.Id,
                ApplicationUserId = loggedInUser!.Id,
                IsPost = vm.IsPost // ✅ Thêm trạng thái IsPost
            };

            if (!string.IsNullOrEmpty(vm.Title))
            {
                var slugHelper = new SlugHelper();
                string slug = slugHelper.GenerateSlug(vm.Title.Trim());
                post.Slug = slug + "-" + Guid.NewGuid();
            }

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            _context.posts!.Add(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Created Successfully!");
            return RedirectToAction("Index");
        }


        [HttpPost]
		public async Task<IActionResult> DeletePost(int id)
		{
			var post = await _context.posts!.Include(x => x.Comments).SingleOrDefaultAsync(x => x.Id == id);

			var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
			var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

			if (loggedInUserRole[0] == WebsiteRole.WebisteAdmin || loggedInUser?.Id == post?.ApplicationUserId)
			{
				_context.posts!.Remove(post!);
				await _context.SaveChangesAsync();
				_notification.Success("Post Deleted Successfully!");
				return RedirectToAction("Index", "Post", new { area = "Admin" });
			}
			else
			{
				_notification.Error("You are not Authorized!");
				return RedirectToAction("Index", "Post", new { area = "Admin" });
			}
		}
        [HttpGet("EditPost")]
        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.posts!
                .Include(p => p.Tag)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                _notification.Error("Post not found!");
                return View();
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);
            if (loggedInUserRole[0] != WebsiteRole.WebisteAdmin && loggedInUser!.Id != post.ApplicationUserId)
            {
                _notification.Error("You Are Not Author Of This Post!");
                return RedirectToAction("Index");
            }

            var vm = new CreatPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                TagName = post.Tag != null ? post.Tag.Name : "",
                WritingPhaseID = post.WritingPhaseID,
                CreatedDate = post.CreatedDate, // Giữ nguyên ngày tạo cũ
                ModifiedDate = DateTime.Now, // Cập nhật ngày sửa đổi
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
                IsPost = post.IsPost, // ✅ Thêm trạng thái IsPost

                // ✅ Thêm danh sách WritingPhases
                WritingPhases = await _context.writingPhases!.ToListAsync()
            };

            return View(vm);
        }

        [HttpPost("EditPost")]
        public async Task<IActionResult> EditPost(CreatPostVM vm)
        {
            if (!ModelState.IsValid)
            {
                // ✅ Load lại danh sách WritingPhases để hiển thị trong View
                vm.WritingPhases = await _context.writingPhases!.ToListAsync();
                return View(vm);
            }

            var post = await _context.posts!.SingleOrDefaultAsync(x => x.Id == vm.Id);
            var tag = await _context.tags!.SingleOrDefaultAsync(t => t.Name == vm.TagName);
            if (post == null)
            {
                _notification.Error("Post not found!");
                return View();
            }

            if (tag == null)
            {
                tag = new Tag()
                {
                    Name = vm.TagName!.ToUpper()
                };
                _context.tags!.Add(tag);
                await _context.SaveChangesAsync();
            }

            post.Title = vm.Title;
            post.Tag = tag;
            post.Description = vm.Description;
            post.WritingPhaseID = vm.WritingPhaseID; // ✅ Cập nhật WritingPhaseID
            post.ModifiedDate = DateTime.Now; // ✅ Cập nhật thời gian sửa đổi
            post.IsPost = vm.IsPost; // ✅ Cập nhật trạng thái IsPost

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail!);
            }

            await _context.SaveChangesAsync();
            _notification.Success("Post Updated Successfully!");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

        [HttpPost("TogglePostStatus")]
        public async Task<IActionResult> TogglePostStatus([FromBody] int id)
        {
            var post = await _context.posts!.FindAsync(id);
            if (post == null)
            {
                return Json(new { success = false });
            }
            post.IsPost = !post.IsPost;
            post.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, newStatus = post.IsPost });
        }


        [HttpGet]
		public IActionResult AutocompleteTags(string keyword)
		{
			var tags = _context.tags!
				.Where(t => t.Name!.StartsWith(keyword))
				.Select(t => t.Name)
				.ToList();

			return Json(tags);
		}

		public string UploadImage(IFormFile file)
		{
			string uniqueFileName = "";
			var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
			uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			var filePath = Path.Combine(folderPath, uniqueFileName);
			using (FileStream fileStream = System.IO.File.Create(filePath))
			{
				file.CopyTo(fileStream);
			}
			return uniqueFileName;
		}
	}
}
