using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace tremorur.Services;

public static class SettingsService
{
    public static List<Alarm> Alarms
    {
        get => GetClassFromStorage<List<Alarm>>("Alarms").ToList();//l�ser liser af alarmer fra storage og h�ndterer JSON-fil
        set => SetClassInStorage("Alarms", value);//skriver ny liste af alarmer til storage JSON-fil 
    }
    private static ImmutableDictionary<string, object?> _cache = ImmutableDictionary<string, object?>.Empty;//undg�r un�dvendig l�sning af data
    private static IPreferences DefaultPreferences => Microsoft.Maui.Storage.Preferences.Default;//bruger MAUI Preferences til at l�se og lagre data
    private static IPreferences Preferences { get; set; } = DefaultPreferences;//har som standard DefaultPreferences men kan overskrives
    
    public static T GetClassFromStorage<T>(string storageKey, T defaultValue)//Henter v�rdi fra storage, med default hvis v�rdien ikke deserialiseres
    {
        if (_cache.TryGetValue(storageKey, out var weaklyTypedCachedValue)//tjekker om _cache er typen T
            && weaklyTypedCachedValue is T value)
            return value;


        var serializedValue = Preferences.Get(storageKey, "");//

        if (serializedValue.Length == 0)
            return defaultValue;

        try
        {
            value = JsonSerializer.Deserialize<T>(serializedValue)!; //
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

    public static void SetClassInStorage<T>(string storageKey, T value) //serialiserer object til JSON
    {
        Preferences.Set(storageKey, JsonSerializer.Serialize(value));//gemmer i Preferences
        ImmutableInterlocked.AddOrUpdate(ref _cache, storageKey, value, (_, _) => value);//opdaterer _cache
    }
}
