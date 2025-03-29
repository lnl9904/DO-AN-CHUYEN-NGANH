namespace BlogWebsite.ViewModels.AssignPost
{
    public class UserAssignmentViewModel
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public int MaxAssignable { get; set; } // Số bài có thể nhận
    }
}
