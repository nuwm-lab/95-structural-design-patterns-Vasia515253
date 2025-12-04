using System;

namespace LabWork
{
    /// <summary>
    /// 1. Цільовий інтерфейс (Target): Очікуваний інтерфейс для клієнта (робота з XML).
    /// </summary>
    public interface IXmlConverter
    {
        string ConvertToXml(string data);
    }

    /// <summary>
    /// 2. Адаптований клас (Adaptee): Існуючий клас з несумісним інтерфейсом (робота з JSON).
    /// </summary>
    public class JsonService
    {
        public string GetJsonData(string input)
        {
            // У реальному житті тут може бути отримання даних або складна логіка,
            // що повертає JSON-рядок.
            Console.WriteLine($"JsonService: Отримано вхідні дані: '{input}'. Повертається JSON.");
            return "{ \"name\": \"Data\", \"value\": \"" + input + "\" }";
        }
    }

    /// <summary>
    /// 3. Адаптер (Adapter): Перекладач, який конвертує JSON в XML.
    /// Реалізує цільовий інтерфейс (IXmlConverter) і використовує адаптований клас (JsonService).
    /// </summary>
    public class JsonToXmlAdapter : IXmlConverter
    {
        private readonly JsonService _jsonService;

        public JsonToXmlAdapter(JsonService jsonService)
        {
            // Принцип інкапсуляції: поле _jsonService є private, і доступ до нього
            // контролюється через конструктор.
            _jsonService = jsonService;
        }

        public string ConvertToXml(string inputData)
        {
            // Крок 1: Виклик методу адаптованого класу для отримання JSON.
            string jsonData = _jsonService.GetJsonData(inputData);

            // Крок 2: Логіка конвертації JSON в XML.
            // *Зверніть увагу: Це спрощена симуляція конвертації.*
            string xmlData = SimulateJsonToXmlConversion(jsonData);

            Console.WriteLine($"Adapter: Конвертовано JSON в XML.");
            return xmlData;
        }

        // Спрощений метод конвертації для прикладу
        private string SimulateJsonToXmlConversion(string json)
        {
            // Приклад конвертації: "{ "name": "Data", "value": "test" }" -> "<root><name>Data</name><value>test</value></root>"
            string xml = json
                .Replace("{", "<root>")
                .Replace("}", "</root>")
                .Replace(":", ">")
                .Replace(",", "</root><root>")
                .Replace("\"", "");
            
            // Очищення та форматування для кращого вигляду (дуже спрощено)
            xml = xml.Replace("<root>", Environment.NewLine + "  <");
            xml = xml.Replace("</root>", "  </");
            xml = xml.Replace(">", ">" + Environment.NewLine);
            xml = xml.Replace("  <  ", "  <");
            xml = xml.Replace("  </  ", "  </");

            return $"<DataExchange>{xml}</DataExchange>";
        }
    }
    
    // --- Основний клас програми ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Демонстрація патерну Адаптер: JSON -> XML ---");
            Console.WriteLine("Натисніть будь-яку клавішу для початку...");
            Console.ReadKey(true);
            Console.WriteLine("---");

            // 1. Створення екземпляру існуючої (адаптованої) служби JSON.
            JsonService jsonService = new JsonService();

            // 2. Створення екземпляру Адаптера, передаючи йому об'єкт JsonService.
            // Адаптер дозволяє нам викликати JsonService через інтерфейс IXmlConverter.
            IXmlConverter adapter = new JsonToXmlAdapter(jsonService);

            // 3. Клієнтський код, який очікує IXmlConverter, тепер може працювати з JsonService
            // завдяки нашому адаптеру.
            string clientInput = "Лабораторна робота ООП";
            Console.WriteLine($"Клієнт: Запитую конвертацію даних: '{clientInput}'");

            // Виклик методу цільового інтерфейсу
            string xmlResult = adapter.ConvertToXml(clientInput);

            Console.WriteLine("---");
            Console.WriteLine("Клієнт: Отриманий XML-результат:");
            Console.WriteLine(xmlResult);
            Console.WriteLine("---");

            // Перевірка, якби ми викликали JsonService безпосередньо:
            // string jsonDirect = jsonService.GetJsonData(clientInput);
            // Console.WriteLine($"JsonService напряму: {jsonDirect}");
        }
    }
}
