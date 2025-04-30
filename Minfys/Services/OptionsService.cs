using System.IO;
using System.Security;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Minfys.Services;

public class OptionsService : IOptionsService
{
    private readonly ILogger<OptionsService> _logger;
    private readonly IMessageService _messageService;
    private readonly string _filePath;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public OptionsService(string userConfigPath, IMessageService messageService, ILogger<OptionsService> logger)
    {
        _messageService = messageService;
        _logger = logger;
        _filePath = userConfigPath;

        _logger.LogInformation("{Service} created", nameof(OptionsService));
    }

    public TOptions? Load<TOptions>(string? sectionKey = null) where TOptions : class, new()
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var rootElement = JsonDocument.Parse(json).RootElement;

            // Если указан ключ секции, пытаемся найти её
            if (!string.IsNullOrEmpty(sectionKey) && rootElement.TryGetProperty(sectionKey, out var section))
            {
                // Десериализуем только указанную секцию
                return JsonSerializer.Deserialize<TOptions>(section.GetRawText(), _serializerOptions);
            }
            else
            {
                // Если секция не указана или не найдена, десериализуем весь документ
                return JsonSerializer.Deserialize<TOptions>(json, _serializerOptions);
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Incorrect file path: {Path}", _filePath);
            _messageService.ShowError($"Invalid path: {_filePath}");
        }
        catch (PathTooLongException ex)
        {
            _logger.LogError(ex, "Path too long: {Path}", _filePath);
            _messageService.ShowError($"Path too long: {_filePath}");
        }
        catch (DirectoryNotFoundException ex)
        {
            _logger.LogError(ex, "Directory does not exist: {Path}", _filePath);
            _messageService.ShowError($"Directory does not exist: {_filePath}");
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogError(ex, "File not found: {Path}", _filePath);
            _messageService.ShowError($"File not found: {_filePath}");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "No read permissions for file: {Path}", _filePath);
            _messageService.ShowError("No read permissions for file");
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error reading from file: {Path}", _filePath);
            _messageService.ShowError("Error reading from file");
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "Unsupported path format: {Path}", _filePath);
            _messageService.ShowError("Unsupported path format");
        }
        catch (SecurityException ex)
        {
            _logger.LogError(ex, "Security error while accessing file: {Path}", _filePath);
            _messageService.ShowError("Security error while accessing file");
        }

        return null;
    }

    public void Save<TOptions>(TOptions options, string? sectionKey = null) where TOptions : class
    {
        try
        {
            // Если секция не указана, просто сохраняем весь объект
            if (string.IsNullOrEmpty(sectionKey))
            {
                var json = JsonSerializer.Serialize(options, _serializerOptions);
                File.WriteAllText(_filePath, json);
                return;
            }

            // Если секция указана, мы должны сохранить её в существующий файл
            JsonDocument? document = null;
            Dictionary<string, JsonElement> rootDict = new();

            // Пытаемся прочитать существующий файл
            if (File.Exists(_filePath))
            {
                var existingJson = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    document = JsonDocument.Parse(existingJson);

                    // Копируем все существующие секции в словарь
                    foreach (var property in document.RootElement.EnumerateObject())
                    {
                        rootDict[property.Name] = property.Value.Clone();
                    }
                }
            }

            // Сериализуем наш объект
            var optionsJson = JsonSerializer.Serialize(options, _serializerOptions);
            var optionsElement = JsonDocument.Parse(optionsJson).RootElement;

            // Заменяем или добавляем нашу секцию
            rootDict[sectionKey] = optionsElement;

            // Собираем финальный JSON
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            writer.WriteStartObject();

            foreach (var kvp in rootDict)
            {
                writer.WritePropertyName(kvp.Key);
                kvp.Value.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.Flush();

            var finalJson = Encoding.UTF8.GetString(stream.ToArray());
            File.WriteAllText(_filePath, finalJson);

            // Освобождаем ресурсы
            document?.Dispose();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Incorrect file path: {Path}", _filePath);
            _messageService.ShowError($"Invalid path: {_filePath}");
        }
        catch (PathTooLongException ex)
        {
            _logger.LogError(ex, "Path too long: {Path}", _filePath);
            _messageService.ShowError($"Path too long: {_filePath}");
        }
        catch (DirectoryNotFoundException ex)
        {
            _logger.LogError(ex, "Directory does not exist: {Path}", _filePath);
            _messageService.ShowError($"Directory does not exist: {_filePath}");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "No write permissions for file: {Path}", _filePath);
            _messageService.ShowError("No write permissions for file");
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error writing to file: {Path}", _filePath);
            _messageService.ShowError("Error writing to file");
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "Unsupported path format: {Path}", _filePath);
            _messageService.ShowError("Unsupported path format");
        }
        catch (SecurityException ex)
        {
            _logger.LogError(ex, "Security error while accessing file: {Path}", _filePath);
            _messageService.ShowError("Security error while accessing file");
        }
    }
}