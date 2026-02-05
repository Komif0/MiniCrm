namespace Chepeno;

using System;
using System.IO;
using System.Text.Json;

public class ClientBaseCleaner
{
    public bool ClearAllClients(string filePath = "clients.json")
    {
        try
        {

            string emptyJson = JsonSerializer.Serialize(new object[0],
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(filePath, emptyJson);

            Console.WriteLine("База данных клиентов (clients.json) успешно очищена.");
            return true;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Файл {filePath} не найден.");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Нет прав доступа к файлу {filePath}.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при очистке базы данных: {ex.Message}");
            return false;
        }
    }
}