using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using ResourceShortageManager.Models;
using System.Text.Json.Serialization;

namespace ResourceShortageManager.Utilities;

public static class FileManager
{
    private static JsonSerializerOptions serializerOptions = new()
    {
        Converters =
        {
                new JsonStringEnumConverter()
        },
        WriteIndented = true
    };

    public static Dictionary<ShortageKey, Shortage> DeserializeShortages(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new Dictionary<ShortageKey, Shortage>();
        }

        string json = File.ReadAllText(filePath);
        List<Shortage>? shortages = JsonSerializer.Deserialize<List<Shortage>>(json, serializerOptions);

        if (shortages is null)
        {
            return new Dictionary<ShortageKey, Shortage>();
        }

        Dictionary<ShortageKey, Shortage> shortagesDictionary = new();

        foreach (Shortage shortage in shortages)
        {
            ShortageKey key = new ShortageKey(shortage.Title, shortage.Category);
            shortagesDictionary[key] = shortage;
        }

        return shortagesDictionary;
    }
    public static void SerializeShortages(string filePath, Dictionary<ShortageKey, Shortage> shortages)
    {
        List<Shortage> shortagesList = shortages.Values.ToList();
        string json = JsonSerializer.Serialize(shortagesList, serializerOptions);
        File.WriteAllText(filePath, json);
    }
}
