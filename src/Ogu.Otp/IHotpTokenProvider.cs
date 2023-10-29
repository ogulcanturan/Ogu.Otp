namespace Ogu.Otp
{
    public interface IHotpTokenProvider
    {
        string GetUri(string audience, string secretKey, long counter = 0);
        string GenerateSecretKey();
        string GenerateCode(string secretKey, long counter = 0, string modifier = null);
        OtpValidationResult ValidateCode(string code, string secretKey, long counter, string modifier = null);
    }
}