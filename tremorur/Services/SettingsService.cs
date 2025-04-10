using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace tremorur.Services;

public static class SettingsService
{
    public static List<Alarm> Alarms
    {
        get => GetClassFromStorage<List<Alarm>>("Alarms").ToList();
        set => SetClassInStorage("Alarms", value);
    }


    private static ImmutableDictionary<string, object?> _cache = ImmutableDictionary<string, object?>.Empty;
    private static IPreferences DefaultPreferences => Microsoft.Maui.Storage.Preferences.Default;
    private static IPreferences Preferences { get; set; } = DefaultPreferences;
    public static T GetClassFromStorage<T>(string storageKey, T defaultValue)
    {
        if (_cache.TryGetValue(storageKey, out var weaklyTypedCachedValue)
            && weaklyTypedCachedValue is T value)
            return value;


        var serializedValue = Preferences.Get(storageKey, "");

        if (serializedValue.Length == 0)
            return defaultValue;

        try
        {
            value = JsonSerializer.Deserialize<T>(serializedValue)!;
            ImmutableInterlocked.AddOrUpdate(ref _cache, storageKey, value, (_, _) => value);

            return value;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T GetClassFromStorage<T>(string storageKey)
        where T : class, new()
        => GetClassFromStorage(storageKey, new T());

    public static void SetClassInStorage<T>(string storageKey, T value)
    {
        Preferences.Set(storageKey, JsonSerializer.Serialize(value));
        ImmutableInterlocked.AddOrUpdate(ref _cache, storageKey, value, (_, _) => value);
    }
}
