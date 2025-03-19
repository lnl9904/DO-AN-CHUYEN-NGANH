namespace BlogWebsite.ViewModels.WritingPhases
{
    public class CreateWritingPhasesVM
    {
        public string? Title { get; set; }
        public int AmountArticles { get; set; }
        public DateTime StartDate { get; set; } // Đợt viết bài
        public DateTime EndDate { get; set; } // Đợt viết bài
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; }
        public bool Is_Opening_registration { get; set; }
        public int RegistrationPeriodID { get; set; }
    }
}
