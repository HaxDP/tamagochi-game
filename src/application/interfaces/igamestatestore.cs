using ConsoleTamagotchi.Application.Models;

namespace ConsoleTamagotchi.Application.Interfaces;

public interface IGameStateStore
{
    bool Exists();
    GameState Load();
    void Save(GameState state);
    void Delete();
}
