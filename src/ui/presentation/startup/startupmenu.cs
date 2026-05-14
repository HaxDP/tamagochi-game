using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Domain.ValueObjects;

namespace ConsoleTamagotchi.Presentation.Startup;

public sealed class StartupMenu
{
    private readonly IGameStateStore _stateStore;

    public StartupMenu(IGameStateStore stateStore)
    {
        _stateStore = stateStore;
    }

    public GameStartSpec Show()
    {
        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("=== Console Tamagotchi ===");
        Console.WriteLine();

        if (_stateStore.Exists())
        {
            var mode = AskChoice("Save found. Continue previous game?", ["1. Continue", "2. New game"], 1, 2);
            if (mode == 1)
            {
                var state = _stateStore.Load();
                return new GameStartSpec(
                    Pet.Restore(state.Pet),
                    GameWorld.Restore(state.World),
                    state.Settings,
                    state.ElapsedSeconds);
            }
        }

        var name = AskText("Pet name", "Mochi");
        var speciesChoice = AskChoice("Choose animal", ["1. Cat", "2. Dog"], 1, 2);
        var species = speciesChoice == 1 ? PetSpecies.Cat : PetSpecies.Dog;
        var growthRate = AskDouble("Growth speed (0.5-3.0)", 1.0, 0.5, 3.0);
        var statDecay = AskDouble("Stat decrease speed (0.3-3.0)", 1.0, 0.3, 3.0);
        var pet = Pet.CreateNew(name, species, new Vector2(10, 5));
        var world = new GameWorld();
        return new GameStartSpec(pet, world, new GameSettings(growthRate, statDecay), 0);
    }

    private static string AskText(string label, string fallback)
    {
        Console.Write($"{label} [{fallback}]: ");
        var text = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(text))
        {
            return fallback;
        }

        return text.Trim();
    }

    private static int AskChoice(string label, IReadOnlyList<string> options, int min, int max)
    {
        while (true)
        {
            Console.WriteLine(label);
            foreach (var option in options)
            {
                Console.WriteLine(option);
            }

            Console.Write($"Pick {min}-{max}: ");
            var raw = Console.ReadLine();
            if (int.TryParse(raw, out var choice) && choice >= min && choice <= max)
            {
                return choice;
            }

            Console.WriteLine("Invalid choice.");
        }
    }

    private static double AskDouble(string label, double fallback, double min, double max)
    {
        while (true)
        {
            Console.Write($"{label} [{fallback:0.0}]: ");
            var raw = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return fallback;
            }

            if (double.TryParse(raw, out var value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Enter a number from {min:0.0} to {max:0.0}.");
        }
    }
}
