using System;
using System.Collections.Generic;
using System.Linq;

class StoryGame
{
    class Scene
    {
        public string Description { get; set; }
        public Dictionary<string, string> Options { get; set; } // Текст выбора -> ID следующей сцены
        public bool IsEnding { get; set; }
    }

    static Dictionary<string, Scene> _world = new();

    static void Main()
    {
        InitGame();
        string currentId = "entry";

        while (true)
        {
            var scene = _world[currentId];
            Console.Clear();
            Console.WriteLine($"--- {currentId.ToUpper()} ---\n");
            Console.WriteLine(scene.Description);

            if (scene.IsEnding) break;

            var optionsList = scene.Options.Keys.ToList();
            for (int i = 0; i < optionsList.Count; i++)
                Console.WriteLine($"{i + 1}. {optionsList[i]}");

            Console.Write("\nВыбор: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= optionsList.Count)
            {
                currentId = scene.Options[optionsList[choice - 1]];
            }
        }
        Console.WriteLine("\n[Нажмите любую клавишу для выхода]");
        Console.ReadKey();
    }

    static void InitGame()
    {
        // Ветка 1: Вход
        _world["entry"] = new Scene
        {
            Description = "Вы стоите перед заброшенным бункером. Внутри слышен гул.",
            Options = new()
            {
                ["Спуститься внутрь"] = "corridor",
                ["Осмотреть окрестности"] = "forest"
            }
        };

        // Ветка 2: Лес (Поиск ресурсов)
        _world["forest"] = new Scene
        {
            Description = "В лесу вы находите старый рюкзак и ржавый люк.",
            Options = new()
            {
                ["Открыть люк"] = "lab",
                ["Вернуться к входу в бункер"] = "entry"
            }
        };

        // Ветка 3: Коридор
        _world["corridor"] = new Scene
        {
            Description = "Длинный коридор. Слева — дверь с надписью 'BIO', справа — лестница вниз.",
            Options = new()
            {
                ["Зайти в BIO"] = "lab",
                ["Спуститься по лестнице"] = "generator"
            }
        };

        // Ветка 4: Лаборатория (Ловушка или прогресс)
        _world["lab"] = new Scene
        {
            Description = "Здесь колбы с синим газом. Одна из них разбита!",
            Options = new()
            {
                ["Задержать дыхание и бежать"] = "generator",
                ["Исследовать записи"] = "death_gas"
            }
        };

        // Финалы
        _world["generator"] = new Scene
        {
            Description = "Вы нашли работающий генератор и активировали связь. Спасение близко!",
            IsEnding = true
        };

        _world["death_gas"] = new Scene
        {
            Description = "Газ оказался быстрее. Вы засыпаете навсегда.",
            IsEnding = true
        };
    }
}