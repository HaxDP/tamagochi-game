using ConsoleTamagotchi.Domain.ValueObjects;

namespace ConsoleTamagotchi.Domain.Models;

public sealed record GameWorldState(
    List<Vector2> Poops,
    List<WorldParticle> Particles,
    double NextPoopSpawnIn,
    double NextParticleSpawnIn);
