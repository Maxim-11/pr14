namespace Class.Models
{
    public class Submission
    {
        public int SubmissionID { get; set; }
        public int AssignmentID { get; set; }
        public int UserID { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string FilePath { get; set; }
        public int? Grade { get; set; }
    }
}
