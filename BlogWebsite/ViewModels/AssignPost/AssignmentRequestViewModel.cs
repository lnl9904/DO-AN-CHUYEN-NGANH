using System.ComponentModel.DataAnnotations;

namespace BlogWebsite.ViewModels.AssignPost
{
    public class AssignmentRequestViewModel
    {
        [Required]
        public string? UserId { get; set; } 

        [Required]
        public int WritingPhaseId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Article count must be at least 1")]
        public int ArticleCount { get; set; }
    }
}
