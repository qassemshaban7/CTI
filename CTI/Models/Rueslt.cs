namespace CTI.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        public string Answer { get; set; }

        public string UserId { get; set; }

        public Question Question { get; set; }
    }
}