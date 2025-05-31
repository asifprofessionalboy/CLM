namespace CLMSAPP.Models
{
    public class UserLoginMember
    {
        public Guid Id { get; set; }
        public Guid MasterId { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string ResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
