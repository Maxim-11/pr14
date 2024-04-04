namespace Class.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int? SenderID { get; set; }
        public int? RecipientID { get; set; }
        public int? AssignmentID { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? Timestamp { get; set; }

    }
}
