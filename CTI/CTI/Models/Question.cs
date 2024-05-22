namespace CTI.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionName { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

        public ICollection<Answer> Answers { get; set; }
        public ICollection<Result> Results { get; set; }
    }
}