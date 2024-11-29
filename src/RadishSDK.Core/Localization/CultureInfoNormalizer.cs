using System.Globalization;

namespace Radish.Localization;

// ReSharper disable once UnusedType.Global
internal static class CultureInfoNormalizer
{
    static CultureInfoNormalizer()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }
}