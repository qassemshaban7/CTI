namespace CTI.ViewModels
{
    public class EditQuestionVM
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
        public List<EditAnswerVM> Answers { get; set; } = new List<EditAnswerVM>();
    }
}
