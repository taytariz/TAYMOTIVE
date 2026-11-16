
using System.Text.Json;

namespace TaymotiveController.Data
{
    public class FileHandler<T>
    {
        private readonly string _filePath;

        public FileHandler(string filePath)
        {
            _filePath = filePath;
        }

        public List<T> Read()
        {
            if (!File.Exists(_filePath))
            {
                return new List<T>();
            }

            string json = File.ReadAllText(_filePath);

            return JsonSerializer.Deserialize<List<T>>(json)
                   ?? new List<T>();
        }

        public void Save(List<T> data)
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_filePath, json);
        }
    }
}
