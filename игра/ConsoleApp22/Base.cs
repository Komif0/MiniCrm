using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Модель сцены для JSON
[JsonStorage("scenes.json")]
public class SceneData : JsonEntity
{
    public string SceneId { get; set; }  // Уникальный идентификатор сцены
    public string Description { get; set; }
    public Dictionary<string, string> Options { get; set; } = new(); // Текст выбора -> ID следующей сцены
    public bool IsEnding { get; set; }
    public string Author { get; set; }  // Автор сцены (для будущих редакторов)
    public string Tags { get; set; }    // Теги для категоризации
}

// Модель сохранения игры
[JsonStorage("saves.json")]
public class GameSave : JsonEntity
{
    public string SaveName { get; set; }
    public string CurrentSceneId { get; set; }
    public DateTime SavedAt { get; set; }
    public int PlayTimeMinutes { get; set; }
    public List<string> VisitedScenes { get; set; } = new(); // История посещенных сцен
}

// Базовый класс для JSON-сущностей
public abstract class JsonEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// Атрибут для указания имени файла
[AttributeUsage(AttributeTargets.Class)]
public class JsonStorageAttribute : Attribute
{
    public string FileName { get; }
    public JsonStorageAttribute(string fileName)
    {
        FileName = fileName;
    }
}

// Основной класс базы данных
public class JsonDatabase
{
    private readonly string _basePath;
    private readonly JsonSerializerOptions _options;

    public JsonDatabase(string basePath = "GameData")
    {
        _basePath = basePath;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    private string GetFilePath<T>() where T : JsonEntity
    {
        var type = typeof(T);
        var attribute = type.GetCustomAttributes(typeof(JsonStorageAttribute), false)
            .FirstOrDefault() as JsonStorageAttribute;

        var fileName = attribute?.FileName ?? $"{type.Name}s.json";
        return Path.Combine(_basePath, fileName);
    }

    public List<T> LoadAll<T>() where T : JsonEntity
    {
        var filePath = GetFilePath<T>();

        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }

    public void SaveAll<T>(List<T> items) where T : JsonEntity
    {
        var filePath = GetFilePath<T>();
        var json = JsonSerializer.Serialize(items, _options);
        File.WriteAllText(filePath, json);
    }

    public T GetById<T>(int id) where T : JsonEntity
    {
        var items = LoadAll<T>();
        return items.FirstOrDefault(x => x.Id == id);
    }

    public T Add<T>(T item) where T : JsonEntity
    {
        var items = LoadAll<T>();
        item.Id = items.Count > 0 ? items.Max(x => x.Id) + 1 : 1;
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        items.Add(item);
        SaveAll(items);
        return item;
    }

    public bool Update<T>(T item) where T : JsonEntity
    {
        var items = LoadAll<T>();
        var index = items.FindIndex(x => x.Id == item.Id);

        if (index == -1) return false;

        item.UpdatedAt = DateTime.Now;
        items[index] = item;
        SaveAll(items);
        return true;
    }

    public bool Delete<T>(int id) where T : JsonEntity
    {
        var items = LoadAll<T>();
        var item = items.FirstOrDefault(x => x.Id == id);

        if (item == null) return false;

        items.Remove(item);
        SaveAll(items);
        return true;
    }

    // Специальный метод для поиска сцены по SceneId
    public SceneData GetSceneBySceneId(string sceneId)
    {
        var scenes = LoadAll<SceneData>();
        return scenes.FirstOrDefault(s => s.SceneId == sceneId);
    }

    // Получить все сцены в виде словаря для удобства
    public Dictionary<string, SceneData> GetScenesDictionary()
    {
        var scenes = LoadAll<SceneData>();
        return scenes.ToDictionary(s => s.SceneId, s => s);
    }
}

class StoryGame
{
    private JsonDatabase _database;
    private Dictionary<string, SceneData> _world;
    private List<string> _visitedScenes;
    private GameSave _currentSave;

    public StoryGame()
    {
        _database = new JsonDatabase("StoryGameData");
        _visitedScenes = new List<string>();
    }

    static void Main()
    {
        var game = new StoryGame();
        game.ShowMainMenu();
    }

    void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ТЕКСТОВАЯ ИГРА С JSON БАЗОЙ ===\n");
            Console.WriteLine("1. Новая игра");
            Console.WriteLine("2. Загрузить игру");
            Console.WriteLine("3. Редактор сцен");
            Console.WriteLine("4. Выход");
            Console.Write("\nВыбор: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    NewGame();
                    break;
                case "2":
                    LoadGame();
                    break;
                case "3":
                    SceneEditor();
                    break;
                case "4":
                    return;
            }
        }
    }

    void NewGame()
    {
        // Загружаем сцены из JSON
        _world = _database.GetScenesDictionary();

        if (_world.Count == 0)
        {
            Console.WriteLine("\nБаза сцен пуста! Создайте сцены в редакторе.");
            Console.ReadKey();
            return;
        }

        Console.Write("\nВведите название сохранения: ");
        var saveName = Console.ReadLine();

        _currentSave = new GameSave
        {
            SaveName = saveName,
            CurrentSceneId = "entry",
            PlayTimeMinutes = 0
        };

        _visitedScenes.Clear();
        PlayGame();
    }

    void PlayGame()
    {
        var startTime = DateTime.Now;

        while (true)
        {
            if (!_world.ContainsKey(_currentSave.CurrentSceneId))
            {
                Console.WriteLine($"\nОшибка: Сцена '{_currentSave.CurrentSceneId}' не найдена!");
                break;
            }

            var scene = _world[_currentSave.CurrentSceneId];

            // Добавляем в историю
            if (!_visitedScenes.Contains(scene.SceneId))
            {
                _visitedScenes.Add(scene.SceneId);
            }

            Console.Clear();
            Console.WriteLine($"=== {scene.SceneId.ToUpper()} ===\n");
            Console.WriteLine(scene.Description);

            if (scene.IsEnding)
            {
                Console.WriteLine("\n=== КОНЕЦ ИГРЫ ===");

                // Сохраняем финальное состояние
                _currentSave.VisitedScenes = _visitedScenes;
                _currentSave.PlayTimeMinutes = (int)(DateTime.Now - startTime).TotalMinutes;
                _database.Add(_currentSave);

                Console.WriteLine($"\nВремя игры: {_currentSave.PlayTimeMinutes} минут");
                Console.WriteLine($"Посещено сцен: {_visitedScenes.Count}");
                break;
            }

            var optionsList = scene.Options.Keys.ToList();
            for (int i = 0; i < optionsList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {optionsList[i]}");
            }

            // Добавляем дополнительные опции
            Console.WriteLine("\n0. Сохранить игру");
            Console.WriteLine("9. Главное меню");

            Console.Write("\nВыбор: ");
            var input = Console.ReadLine();

            if (input == "0")
            {
                QuickSave(startTime);
                continue;
            }
            else if (input == "9")
            {
                return;
            }

            if (int.TryParse(input, out int choice) &&
                choice > 0 && choice <= optionsList.Count)
            {
                var selectedText = optionsList[choice - 1];
                _currentSave.CurrentSceneId = scene.Options[selectedText];
            }
        }

        Console.WriteLine("\n[Нажмите любую клавишу]");
        Console.ReadKey();
    }

    void QuickSave(DateTime startTime)
    {
        _currentSave.VisitedScenes = _visitedScenes;
        _currentSave.PlayTimeMinutes = (int)(DateTime.Now - startTime).TotalMinutes;

        if (_currentSave.Id == 0)
        {
            _database.Add(_currentSave);
        }
        else
        {
            _database.Update(_currentSave);
        }

        Console.WriteLine("\nИгра сохранена!");
        System.Threading.Thread.Sleep(1000);
    }

    void LoadGame()
    {
        var saves = _database.LoadAll<GameSave>();

        if (saves.Count == 0)
        {
            Console.WriteLine("\nНет сохраненных игр!");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("=== ЗАГРУЗКА ИГРЫ ===\n");

        for (int i = 0; i < saves.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {saves[i].SaveName} - {saves[i].SavedAt:dd.MM.yyyy HH:mm} ({saves[i].PlayTimeMinutes} мин)");
        }

        Console.Write("\nВыберите сохранение: ");
        if (int.TryParse(Console.ReadLine(), out int choice) &&
            choice > 0 && choice <= saves.Count)
        {
            _currentSave = saves[choice - 1];
            _world = _database.GetScenesDictionary();
            _visitedScenes = _currentSave.VisitedScenes ?? new List<string>();

            Console.WriteLine($"\nЗагружено: {_currentSave.SaveName}");
            Console.ReadKey();

            PlayGame();
        }
    }

    void SceneEditor()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== РЕДАКТОР СЦЕН ===\n");
            Console.WriteLine("1. Просмотреть все сцены");
            Console.WriteLine("2. Добавить новую сцену");
            Console.WriteLine("3. Редактировать сцену");
            Console.WriteLine("4. Удалить сцену");
            Console.WriteLine("5. Импортировать сцены по умолчанию");
            Console.WriteLine("6. Назад");
            Console.Write("\nВыбор: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewAllScenes();
                    break;
                case "2":
                    AddScene();
                    break;
                case "3":
                    EditScene();
                    break;
                case "4":
                    DeleteScene();
                    break;
                case "5":
                    ImportDefaultScenes();
                    break;
                case "6":
                    return;
            }
        }
    }

    void ViewAllScenes()
    {
        var scenes = _database.LoadAll<SceneData>();

        Console.Clear();
        Console.WriteLine("=== ВСЕ СЦЕНЫ ===\n");

        foreach (var scene in scenes)
        {
            Console.WriteLine($"ID: {scene.SceneId}");
            Console.WriteLine($"Описание: {scene.Description}");
            Console.WriteLine($"Финал: {(scene.IsEnding ? "Да" : "Нет")}");
            Console.WriteLine($"Вариантов выбора: {scene.Options?.Count ?? 0}");
            Console.WriteLine($"Автор: {scene.Author ?? "Неизвестно"}");
            Console.WriteLine(new string('-', 40));
        }

        Console.WriteLine($"\nВсего сцен: {scenes.Count}");
        Console.ReadKey();
    }

    void AddScene()
    {
        Console.Clear();
        Console.WriteLine("=== НОВАЯ СЦЕНА ===\n");

        var scene = new SceneData();

        Console.Write("ID сцены (например, 'entry'): ");
        scene.SceneId = Console.ReadLine();

        Console.Write("Описание: ");
        scene.Description = Console.ReadLine();

        Console.Write("Это финал? (y/n): ");
        scene.IsEnding = Console.ReadLine()?.ToLower() == "y";

        Console.Write("Автор: ");
        scene.Author = Console.ReadLine();

        if (!scene.IsEnding)
        {
            scene.Options = new Dictionary<string, string>();
            Console.WriteLine("\nДобавьте варианты выбора (пустой текст для завершения):");

            while (true)
            {
                Console.Write("Текст выбора: ");
                var optionText = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(optionText)) break;

                Console.Write("ID следующей сцены: ");
                var nextSceneId = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nextSceneId)) break;

                scene.Options[optionText] = nextSceneId;
            }
        }

        _database.Add(scene);
        Console.WriteLine("\nСцена добавлена!");
        Console.ReadKey();
    }

    void EditScene()
    {
        Console.Write("\nВведите ID сцены для редактирования: ");
        var sceneId = Console.ReadLine();

        var scene = _database.GetSceneBySceneId(sceneId);
        if (scene == null)
        {
            Console.WriteLine("Сцена не найдена!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nОставьте поле пустым, чтобы оставить текущее значение.");

        Console.Write($"Новое описание [{scene.Description}]: ");
        var newDesc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDesc))
            scene.Description = newDesc;

        Console.Write($"Автор [{scene.Author}]: ");
        var newAuthor = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAuthor))
            scene.Author = newAuthor;

        _database.Update(scene);
        Console.WriteLine("\nСцена обновлена!");
        Console.ReadKey();
    }

    void DeleteScene()
    {
        Console.Write("\nВведите ID сцены для удаления: ");
        var sceneId = Console.ReadLine();

        var scene = _database.GetSceneBySceneId(sceneId);
        if (scene == null)
        {
            Console.WriteLine("Сцена не найдена!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nУдалить сцену '{sceneId}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _database.Delete<SceneData>(scene.Id);
            Console.WriteLine("Сцена удалена!");
        }

        Console.ReadKey();
    }

    void ImportDefaultScenes()
    {
        var defaultScenes = new List<SceneData>
        {
            new SceneData
            {
                SceneId = "entry",
                Description = "Вы стоите перед заброшенным бункером. Внутри слышен гул.",
                Options = new Dictionary<string, string>
                {
                    ["Спуститься внутрь"] = "corridor",
                    ["Осмотреть окрестности"] = "forest"
                },
                Author = "System",
                Tags = "start"
            },
            new SceneData
            {
                SceneId = "forest",
                Description = "В лесу вы находите старый рюкзак и ржавый люк.",
                Options = new Dictionary<string, string>
                {
                    ["Открыть люк"] = "lab",
                    ["Вернуться к входу в бункер"] = "entry"
                },
                Author = "System"
            },
            new SceneData
            {
                SceneId = "corridor",
                Description = "Длинный коридор. Слева — дверь с надписью 'BIO', справа — лестница вниз.",
                Options = new Dictionary<string, string>
                {
                    ["Зайти в BIO"] = "lab",
                    ["Спуститься по лестнице"] = "generator"
                },
                Author = "System"
            },
            new SceneData
            {
                SceneId = "lab",
                Description = "Здесь колбы с синим газом. Одна из них разбита!",
                Options = new Dictionary<string, string>
                {
                    ["Задержать дыхание и бежать"] = "generator",
                    ["Исследовать записи"] = "death_gas"
                },
                Author = "System"
            },
            new SceneData
            {
                SceneId = "generator",
                Description = "Вы нашли работающий генератор и активировали связь. Спасение близко!",
                IsEnding = true,
                Author = "System"
            },
            new SceneData
            {
                SceneId = "death_gas",
                Description = "Газ оказался быстрее. Вы засыпаете навсегда.",
                IsEnding = true,
                Author = "System"
            }
        };

        foreach (var scene in defaultScenes)
        {
            // Проверяем, нет ли уже такой сцены
            if (_database.GetSceneBySceneId(scene.SceneId) == null)
            {
                _database.Add(scene);
            }
        }

        Console.WriteLine("\nСцены по умолчанию импортированы!");
        Console.ReadKey();
    }
}