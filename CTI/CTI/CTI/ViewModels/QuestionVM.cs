namespace CTI.ViewModels
{
    public class QuestionVM
    {
        public string QuestionName { get; set; }
        public List<AnswerVM> Answers { get; set; } = new List<AnswerVM>();
    }
}
