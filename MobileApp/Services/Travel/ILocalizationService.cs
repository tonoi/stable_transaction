using Microsoft.Extensions.Localization;
using System.Globalization;

namespace JPYCOffline.Services.Travel;

public interface ILocalizationService
{
    string GetString(string key);
    void SetCulture(string cultureCode);
    string GetCurrentCulture();
    List<CultureInfo> GetSupportedCultures();
}

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer<LocalizationService> _localizer;
    private CultureInfo _currentCulture;

    public LocalizationService(IStringLocalizer<LocalizationService> localizer)
    {
        _localizer = localizer;
        _currentCulture = CultureInfo.CurrentCulture;
    }

    public string GetString(string key)
    {
        return _localizer[key];
    }

    public void SetCulture(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        _currentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public string GetCurrentCulture()
    {
        return _currentCulture.Name;
    }

    public List<CultureInfo> GetSupportedCultures()
    {
        return new List<CultureInfo>
        {
            new CultureInfo("en"),
            new CultureInfo("ja"),
            new CultureInfo("fr"),
            new CultureInfo("de"),
            new CultureInfo("ko"),
            new CultureInfo("zh-CN"),
            new CultureInfo("hi")
        };
    }
}
