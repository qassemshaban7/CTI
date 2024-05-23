namespace CTI.ViewModels
{
    public class EditUserVM
    {
        public string Id { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; } // رقم الجوال
        public string Status { get; set; } // حالة المتدرب
        public string TrainingProgram { get; set; }  // البرنامج التدريبي
        public string Email { get; set; }
        public string? EnglishFullName { get; set; }

        public long? RegisterNum { get; set; } // رقم السجل

        public string? Department { get; set; }  //  القسم

    }
}
