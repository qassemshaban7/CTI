namespace CTI.ViewModels
{
    public class EditCourseVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phase { get; set; }
        public string Specialization { get; set; }
        public long ReferenceNumber { get; set; }
        public long Coursecode { get; set; }
        public string Department { get; set; }
        public string TypeDivition { get; set; }

        public string UserId { get; set; }
    }
}
