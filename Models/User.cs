namespace Class.Models
{
    public partial class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string? MiddleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FcmToken { get; set; }

        public string? Salt { get; set; }
        public string UserType { get; set; }
    }
}
