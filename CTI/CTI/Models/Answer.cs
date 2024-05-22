namespace CTI.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string AnswerName { get; set; }    

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public ICollection<Result> Results { get; set; }    
    }
}