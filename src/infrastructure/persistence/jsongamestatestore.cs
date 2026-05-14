using System.Text.Json;
using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Application.Models;

namespace ConsoleTamagotchi.Infrastructure.Persistence;

public sealed class JsonGameStateStore : IGameStateStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public JsonGameStateStore(string filePath)
    {
        _filePath = filePath;
    }

    public bool Exists() => File.Exists(_filePath);

    public GameState Load()
    {
        var json = File.ReadAllText(_filePath);
        var state = JsonSerializer.Deserialize<GameState>(json, SerializerOptions);
        return state ?? throw new InvalidDataException("Save file is corrupted.");
    }

    public void Save(GameState state)
    {
        var directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var json = JsonSerializer.Serialize(state, SerializerOptions);
        File.WriteAllText(_filePath, json);
    }

    public void Delete()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
