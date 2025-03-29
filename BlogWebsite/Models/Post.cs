	namespace BlogWebsite.Models
{
	public class Post
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? ApplicationUserId { get; set; }
		public ApplicationUser? ApplicationUsers { get; set; }
        public int WritingPhaseID { get; set; }
        public WritingPhase? WritingPhases { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; }
        public int ViewCount { get; set; }
		public string? Description { get; set; }
        public string? Download_path { get; set; }
        public decimal Royalty { get; set; }
        public int TagId { get; set; }
		public Tag? Tag { get; set; }
		public string? Slug { get; set; }
		public string? ThumbnailUrl { get; set; }
		public bool IsPost { get; set; } = false;
		public int LikeCount { get; set; }
		public ICollection<Comment>? Comments { get; set; }
		public ICollection<Reaction>? Reactions { get; set; }
	}
}


















































/*Lê Đức Tài, Bùi Ngọc Na*/
