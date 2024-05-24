using System.ComponentModel.DataAnnotations.Schema;

namespace CTI.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }

        public int QuestionType { get; set; } // 1- text 2- one selection 3- multiple selection

        public string? FirstAnswer { get; set; }
        public string? SecondAnswer { get; set; }
        public string? ThirdAnswer { get; set; }
        public string? FourthAnswer { get; set; }
        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        //public ICollection<Answer> Answers { get; set; }
        //public ICollection<Result> Results { get; set; }
    }
}