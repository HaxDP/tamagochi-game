using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.ValueObjects;

namespace ConsoleTamagotchi.Domain.Models;

public sealed record PetState(
    string Name,
    PetSpecies Species,
    PetStage Stage,
    PetAnimationState AnimationState,
    bool IsSleeping,
    bool IsDead,
    double AgeSeconds,
    double StageTimeSeconds,
    double ActionTimeRemaining,
    string StatusMessage,
    Vector2 Position,
    Vector2 Velocity,
    double Hunger,
    double Happiness,
    double Energy,
    double Hygiene,
    double Health);
