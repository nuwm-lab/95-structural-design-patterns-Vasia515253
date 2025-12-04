using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace LabWork
{
    // ====================================================================
    // 1. МОДЕЛІ ДАНИХ (Data Models)
    // ====================================================================
    public class DataModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    // ====================================================================
    // 2. ІНТЕРФЕЙСИ (Interfaces)
    // ====================================================================

    /// <summary>
    /// Цільовий інтерфейс (Target): Інтерфейс, який очікує клієнт (робота з XML).
    /// </summary>
    public interface IXmlConverter
    {
        string ConvertToXml(string jsonData);
    }

    /// <summary>
    /// Інтерфейс для адаптованого класу: Дозволяє інверсію залежностей (DIP).
    /// </summary>
    public interface IJsonService
    {
        string GetJsonData(int id, string input);
    }

    // ====================================================================
    // 3. АДАПТОВАНИЙ КЛАС (Adaptee)
    // ====================================================================

    /// <summary>
    /// Існуючий клас, інтерфейс якого несумісний (працює з JSON).
    /// </summary>
    public class JsonService : IJsonService
    {
        public string GetJsonData(int id, string input)
        {
            // Створення об'єкта моделі
            var data = new DataModel
            {
                Id = id,
                Title = $"Елемент_{id}",
                Content = input
            };

            Console.WriteLine($"JsonService: Отримано вхідні дані: '{input}'. Повертається JSON.");
            
            // Використання System.Text.Json для коректної серіалізації
            return JsonSerializer.Serialize(data);
        }
    }

    // ====================================================================
    // 4. АДАПТЕР (Adapter)
    // ====================================================================

    /// <summary>
    /// Адаптер, який реалізує IXmlConverter і використовує IJsonService,
    /// виконуючи конвертацію JSON -> XML.
    /// </summary>
    public class JsonToXmlAdapter : IXmlConverter
    {
        private readonly IJsonService _jsonService;

        // Dependency Injection: Впровадження IJsonService через конструктор
        // для дотримання принципу інверсії залежностей та інкапсуляції.
        public JsonToXmlAdapter(IJsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public string ConvertToXml(string inputData)
        {
            try
            {
                // Крок 1: Отримання JSON-даних через адаптований сервіс
                string jsonData = _jsonService.GetJsonData(101, inputData);

                // Крок 2: Коректний парсинг JSON у DataModel
                DataModel model = JsonSerializer.Deserialize<DataModel>(jsonData);
                if (model == null)
                {
                    throw new JsonException("Failed to deserialize JSON into DataModel (result is null).");
                }

                // Крок 3: Формування XML за допомогою LINQ to XML
                // Це забезпечує коректне екранування спеціальних символів (наприклад, <, >, &)
                XElement xml = new XElement("DataExchange",
                    new XElement("Record",
                        new XElement("ID", model.Id),
                        new XElement("Title", model.Title),
                        new XElement("Content", model.Content)
                    )
                );

                Console.WriteLine("Adapter: Успішно конвертовано JSON в XML.");
                return xml.ToString();
            }
            catch (JsonException ex)
            {
                // Обробка помилок при некоректному форматі JSON
                Console.WriteLine($"Adapter Error: Помилка парсингу JSON: {ex.Message}");
                return $"<Error Type=\"JsonParsing\">{ex.Message}</Error>";
            }
            catch (Exception ex)
            {
                // Загальна обробка інших помилок
                Console.WriteLine($"Adapter Error: Невідома помилка: {ex.Message}");
                return $"<Error Type=\"General\">{ex.Message}</Error>";
            }
        }
    }

    // ====================================================================
    // 5. ОСНОВНА ПРОГРАМА (Program)
    // ====================================================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Демонстрація структурного патерну Адаптер (JSON -> XML) ---");

            // 1. Ініціалізація залежностей
            // Ми створюємо об'єкт JsonService (Adaptee)
            IJsonService jsonService = new JsonService();

            // 2. Створення Адаптера
            // Клієнт взаємодіє з Адаптером через інтерфейс IXmlConverter (Target)
            IXmlConverter adapter = new JsonToXmlAdapter(jsonService);

            // 3. Клієнтський код, який очікує IXmlConverter
            string clientInput = "Дані для обміну: <тест> & \"опис\"";
            Console.WriteLine($"\nКлієнт: Запитую конвертацію даних: '{clientInput}'");

            // Виклик методу цільового інтерфейсу
            string xmlResult = adapter.ConvertToXml(clientInput);

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Клієнт: Отриманий XML-результат:");
            Console.WriteLine(xmlResult);
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
