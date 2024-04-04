namespace Class.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; }
        public int CourseID { get; set; }
        public string AssignmentName { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public int MaxGrade { get; set; }
    }

}
