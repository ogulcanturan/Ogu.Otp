namespace Ogu.Otp
{
    public class OtpValidationResult
    {
        public OtpValidationResult(bool isValid, int? tolerance = null)
        {
            IsValid = isValid;
            Tolerance = tolerance;
        }
        public bool IsValid { get; }
        public int? Tolerance { get; }
    }
}