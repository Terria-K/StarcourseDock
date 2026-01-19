using Nickel;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Teuria.StartRunPlus;

internal sealed partial class YamlLocalizationProvider : ILocalizationProvider<IReadOnlyList<string>>
{
    private ILocalizationTokenExtractor<string> tokenExtractor;
    private Func<string, Stream?> localeStreamFunction;
    private IDeserializer Deserializer;
    private Dictionary<string, object?> LocalizationCache;

    [GeneratedRegex("{{([ \\w\\.\\-_]+)}}")]
    private static partial Regex TokenRegex();

    public YamlLocalizationProvider(ILocalizationTokenExtractor<string> tokenExtractor, Func<string, Stream?> localeStreamFunction)
    {
        this.tokenExtractor = tokenExtractor;
        this.localeStreamFunction = localeStreamFunction;
        Deserializer = new DeserializerBuilder().Build();
        LocalizationCache = new Dictionary<string, object?>();
    }

    public string? Localize(string locale, IReadOnlyList<string> key, object? tokens = null)
    {
        object? localization = GetLocalization(locale);
        if (localization == null)
        {
            return null;
        }

        return Localize(localization, key, 0, tokens);
    }

    private string? Localize(object localization, IReadOnlyList<string> key, int keyIndex, object? tokens)
    {
        if (keyIndex >= key.Count)
        {
            if (localization is string text)
            {
                return Localize(text, tokens);
            }

            if (localization is List<object> source)
            {
                return Localize(string.Join("\n", source.Select(v => v).OfType<string>()), tokens);
            }

            return null;
        }

        if (localization is Dictionary<object, object> obj)
        {
            return Localize(obj, key, keyIndex, tokens);
        }

        if (localization is List<object> arr)
        {
            return Localize(arr, key, keyIndex, tokens);
        }

        return null;
    }

    private string? Localize(Dictionary<object, object> localization, IReadOnlyList<string> key, int keyIndex, object? tokens)
    {
        object value = localization[key[keyIndex]];
        if (value == null)
        {
            return null;
        }

        return Localize(value, key, keyIndex + 1, tokens);
    }

    private string? Localize(List<object> localization, IReadOnlyList<string> key, int keyIndex, object? tokens)
    {
        if (!int.TryParse(key[keyIndex], out var result) || localization.Count < result)
        {
            return null;
        }

        return Localize(localization.Skip(result).First(), key, keyIndex + 1, tokens);
    }

    private string Localize(string localizationString, object? tokens)
    {
        IReadOnlyDictionary<string, string> tokenLookup = tokenExtractor.ExtractTokens(tokens);
        return TokenRegex().Replace(localizationString, (match) =>
        {
            string key = match.Groups[1].Value.Trim();
            string? value;
            return (!tokenLookup.TryGetValue(key, out value)) ? match.Value : value;
        });
    }

    private object? GetLocalization(string locale)
    {
        if (LocalizationCache.TryGetValue(locale, out object? value))
        {
            return value;
        }

        try
        {
            using Stream? stream = localeStreamFunction(locale);
            if (stream == null)
            {
                return null;
            }

            using StreamReader reader = new StreamReader(stream);
            value = Deserializer.Deserialize(reader) ?? new object();
            LocalizationCache[locale] = value;
            return value;
        }
        catch
        {
            LocalizationCache[locale] = null;
            return value;
        }
    }
}