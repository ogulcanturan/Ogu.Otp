using System.ComponentModel.DataAnnotations;

namespace Otp.Api.Models
{
    public class ValidateOtpRequestModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
