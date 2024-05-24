using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CTI.Models
{
    public class Survey
    {
        public int Id { get; set; }
        [Required]
        public string SurveyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ForWho { get; set; }  // 1-student  2- vistor  3- all
        public int SurveyType { get; set; }  // 1-Course  2- Trainer  3- Service
        public ICollection<Question> Questions { get; set; } 

    }
}