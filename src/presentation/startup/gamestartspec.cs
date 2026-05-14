using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Presentation.Startup;

public sealed record GameStartSpec(
    Pet Pet,
    GameWorld World,
    GameSettings Settings,
    double ElapsedSeconds);
