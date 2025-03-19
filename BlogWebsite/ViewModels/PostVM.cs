
namespace BlogWebsite.ViewModels
{
	public class PostVM
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? TagName { get; set; }
		public int ViewCount { get; set; }
		public string? AuthorName { get; set; }
		public DateTime CreateDate { get; set; }
		public string? ThumbnailUrl { get; set; }
		public int LikeCount { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int WritingPhaseID { get; set; }
        public string? Download_path { get; set; }
        public decimal Royalty { get; set; }
    }
}
