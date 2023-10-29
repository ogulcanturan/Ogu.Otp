namespace Ogu.Otp
{
    public interface ITotpTokenProvider
    {
        string GetUri(string audience, string secretKey);
        string GenerateSecretKey();
        string GenerateCode(string secretKey, string modifier = null);
        OtpValidationResult ValidateCode(string code, string secretKey, string modifier = null);
    }
}