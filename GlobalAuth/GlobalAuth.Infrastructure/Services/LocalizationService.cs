using GlobalAuth.Application.Common;
using Microsoft.Extensions.Localization;
using GlobalAuth.Application.Abstraction;

namespace GlobalAuth.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            var assembly = typeof(ApplicationAssemblyMarker).Assembly;
            _localizer = factory.Create("Message", assembly.GetName().Name!);
        }

        public string this[string key] => _localizer[key];
    }
}
