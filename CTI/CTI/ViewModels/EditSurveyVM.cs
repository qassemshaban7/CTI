namespace CTI.ViewModels
{
    public class EditSurveyVM
    {
        public int SurveyId { get; set; } 
        public string SurveyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ForWho { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public List<EditQuestionVM> Questions { get; set; } = new List<EditQuestionVM>();
    }
}
