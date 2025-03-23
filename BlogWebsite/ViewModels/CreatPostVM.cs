using System.ComponentModel.DataAnnotations;
using BlogWebsite.Models;
namespace BlogWebsite.ViewModels
{
	public class CreatPostVM
	{
		public int Id { get; set; }
		[Required]
		public string? Title { get; set; }
		public int TagId { get; set; }
		public string? TagName { get; set; }
		public string? ApplicationUserId { get; set; }
        public int WritingPhaseID { get; set; }
        public List<BlogWebsite.Models.WritingPhases> WritingPhases { get; set; } = new List<BlogWebsite.Models.WritingPhases>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime UpdatedDate { get; set; }
        public string? Description { get; set; }
		public string? ThumbnailUrl { get; set; }
		public IFormFile? Thumbnail { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? Download_path { get; set; }
        public decimal Royalty { get; set; }
    }
}
