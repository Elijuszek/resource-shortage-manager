using System.Text.Json;
using ResourceShortageManager.Models;
using System.Text.Json.Serialization;

namespace ResourceShortageManager.Utilities
{
    public class FileManager
    {
        private JsonSerializerOptions _serializerOptions;

        public FileManager()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                WriteIndented = true
            };
        }

        public Dictionary<ShortageKey, Shortage> DeserializeShortages(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<ShortageKey, Shortage>();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                string json = reader.ReadToEnd();
                List<Shortage>? shortages = JsonSerializer.Deserialize<List<Shortage>>(json, _serializerOptions);

                if (shortages is null)
                {
                    return new Dictionary<ShortageKey, Shortage>();
                }

                return shortages.ToDictionary(
                    shortage => new ShortageKey(shortage.Title, shortage.Room),
                    shortage => shortage
                );
            }
        }

        public void SerializeShortages(string filePath, Dictionary<ShortageKey, Shortage> shortages)
        {
            List<Shortage> shortagesList = shortages.Values.ToList();
            string json = JsonSerializer.Serialize(shortagesList, _serializerOptions);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
        }
    }
}
