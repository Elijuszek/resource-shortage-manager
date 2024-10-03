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

        public Dictionary<string, Shortage> DeserializeShortages(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, Shortage>();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                string json = reader.ReadToEnd();
                Dictionary<string, Shortage>? shortages = JsonSerializer.Deserialize<Dictionary<string, Shortage>>(json, _serializerOptions);

                if (shortages is null)
                {
                    return new Dictionary<string, Shortage>();
                }

                return shortages;
            }
        }

        public void SerializeShortages(string filePath, Dictionary<string, Shortage> shortages)
        {
            string json = JsonSerializer.Serialize(shortages, _serializerOptions);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
        }

    }
}
