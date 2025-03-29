namespace BlogWebsite.Models
{
    public class WritingPhase
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUsers { get; set; }
        public int AmountArticles { get; set; }
        public DateTime StartDate { get; set; } // Đợt viết bài
        public DateTime EndDate { get; set; } // Đợt viết bài
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; }
        public bool Is_Opening_registration { get; set; }
        public int RegistrationPeriodID { get; set; }
        public RegistrationPeriods? RegistrationPeriods { get; set; }
        public ICollection<Post>? posts { get; set; }
        public string? Slug { get; internal set; }
        public ICollection<UserCapacities>? UserCapacities { get; set; }
    }
}
