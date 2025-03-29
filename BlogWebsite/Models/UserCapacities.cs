namespace BlogWebsite.Models
{
    public class UserCapacities
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUsers { get; set; }
        public int WritingPhaseID { get; set; }
        public WritingPhases? WritingPhases { get; set; }
        public int MaxAssignable { get; set; } // Số bài có thể nhận
    }
}
