namespace BlogWebsite.Models
{
    public class RegistrationPeriods
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime RegisStart { get; set; } // Đợt Đăng Ký
        public DateTime RegisEnd { get; set; } // Đợt Đăng Ký
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; }
        public DateTime RegisDeadlineStart { get; set; } // Hạn đợt Đăng Ký
        public DateTime RegisDeadlineEnd { get; set; }  // Hạn đợt Đăng Ký
        public bool Is_Opening_registration { get; set; }
        public ICollection<WritingPhases>? WritingPhases { get; set; }
    }
}
