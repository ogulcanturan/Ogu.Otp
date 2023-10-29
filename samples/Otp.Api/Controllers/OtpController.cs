using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ogu.Otp;
using Otp.Api.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Otp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private static ICollection<UserModel> _users = new Collection<UserModel>();

        public OtpController() { }

        [HttpGet("generate/totp")]
        public IActionResult GenerateTotp([FromQuery]string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is null or empty!");

            bool isAnyExists = _users.Any(x => string.Equals(x.Username, username, StringComparison.InvariantCultureIgnoreCase));

            if (isAnyExists)
                return BadRequest("Username is already exists");

            var totp = HttpContext.RequestServices.GetRequiredService<TotpTokenProvider>();

            var secretKey = totp.GenerateSecretKey();

            string uri = totp.GetUri(username, secretKey);

            _users.Add(new UserModel()
            {
                Username = username,
                SecretKey = secretKey,
            });

            var qrCodeBytes = GenerateQrCode(uri);

            return File(qrCodeBytes, "image/png");
        }

        [HttpPost("validate/totp")]
        public IActionResult ValidateTotp([FromBody] ValidateOtpRequestModel validateOtpRequest)
        {
            var totp = HttpContext.RequestServices.GetRequiredService<TotpTokenProvider>();

            var user = _users.FirstOrDefault(x => string.Equals(x.Username, validateOtpRequest.Username, StringComparison.InvariantCultureIgnoreCase));

            if (user == null)
            {
                return NotFound($"User with `{validateOtpRequest.Username}` not found");
            }

            var validationResult = totp.ValidateCode(validateOtpRequest.Code, user.SecretKey);

            return Ok(validationResult);
        }

        [HttpGet("generate/hotp")]
        public IActionResult GenerateHotp(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is null or empty!");

            bool isAnyExists = _users.Any(x => x.Username == username);

            if (isAnyExists)
                return BadRequest("Username is already exists");

            var totp = HttpContext.RequestServices.GetRequiredService<HotpTokenProvider>();

            var secretKey = totp.GenerateSecretKey();

            string uri = totp.GetUri(username, secretKey);

            _users.Add(new UserModel()
            {
                Username = username,
                SecretKey = secretKey,
            });

            var qrCodeBytes = GenerateQrCode(uri);

            return File(qrCodeBytes, "image/png");
        }

        [HttpPost("validate/hotp")]
        public IActionResult ValidateHotp([FromBody] ValidateOtpRequestModel validateOtpRequest)
        {
            var hotp = HttpContext.RequestServices.GetRequiredService<HotpTokenProvider>();

            var user = _users.FirstOrDefault(x => string.Equals(x.Username, validateOtpRequest.Username, StringComparison.InvariantCultureIgnoreCase));

            if (user == null)
            {
                return NotFound($"User with `{validateOtpRequest.Username}` not found");
            }

            var validationResult = hotp.ValidateCode(validateOtpRequest.Code, user.SecretKey, user.Counter);

            if (validationResult.IsValid)
            {
                user.Counter += 1;
            }

            return Ok(validationResult);
        }

        private static byte[] GenerateQrCode(string uri)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode sd = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = sd.GetGraphic(5);

            return qrCodeBytes;
        }
    }
}