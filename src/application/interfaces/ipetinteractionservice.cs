using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Application.Interfaces;

public interface IPetInteractionService
{
    void Feed(Pet pet, GameWorld world);
    void Play(Pet pet, GameWorld world);
    void Clean(Pet pet, GameWorld world);
    void Sleep(Pet pet, GameWorld world);
    void Heal(Pet pet, GameWorld world);
}
