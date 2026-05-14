using ConsoleTamagotchi.Domain.ValueObjects;

namespace ConsoleTamagotchi.Domain.Models;

public readonly record struct WorldParticle(Vector2 Position, double TimeToLive, char Symbol);
