namespace GlobalAuth.Application.Common.VerificationOptions
{
    public class VerificationCodeModel
    {
        public string Name { get; set; }
        public int DigitLength { get; set; }
        public int TTL { get; set; }
    }
}
