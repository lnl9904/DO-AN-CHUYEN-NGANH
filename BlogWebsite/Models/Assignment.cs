namespace BlogWebsite.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUsers { get; set; }
        public int WritingPhaseID { get; set; }
        public WritingPhases? WritingPhases { get; set; }
        public int ArticleCount { get; set; } // Số lượng bài viết được giao
    }
}
