namespace Otp.Api.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string SecretKey { get; set; }
        public long Counter { get; set; }
    }
}