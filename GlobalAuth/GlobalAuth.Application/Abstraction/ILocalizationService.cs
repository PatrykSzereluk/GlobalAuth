namespace GlobalAuth.Application.Abstraction
{
    public interface ILocalizationService
    {
        string this[string key] { get; }
    }
}
