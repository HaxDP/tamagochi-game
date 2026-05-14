using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Application.Models;

public sealed record GameState(
    PetState Pet,
    GameWorldState World,
    GameSettings Settings,
    double ElapsedSeconds);
