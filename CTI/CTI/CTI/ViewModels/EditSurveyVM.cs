namespace CTI.ViewModels
{
    public class EditSurveyVM
    {
        public int SurveyId { get; set; } 
        public string SurveyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ForWho { get; set; }
        public int SurveyType { get; set; }
    }
}
