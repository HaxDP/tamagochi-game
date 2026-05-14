using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Application.Interfaces;

public interface IPetSimulationService
{
    void Update(Pet pet, GameWorld world, GameSettings settings, double deltaSeconds, int worldWidth, int worldHeight);
}
