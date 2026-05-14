using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.ValueObjects;

namespace ConsoleTamagotchi.Domain.Models;

public sealed class Pet
{
    private Pet(string name, PetSpecies species, Vector2 startPosition)
    {
        Name = name;
        Species = species;
        Position = startPosition;
        Velocity = new Vector2(1, 0.5);
    }

    public string Name { get; }
    public PetSpecies Species { get; }
    public PetStage Stage { get; private set; } = PetStage.Egg;
    public PetAnimationState AnimationState { get; private set; } = PetAnimationState.Idle;
    public PetNeeds Needs { get; } = new();
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public bool IsSleeping { get; private set; }
    public double AgeSeconds { get; private set; }
    public double StageTimeSeconds { get; private set; }
    public double ActionTimeRemaining { get; private set; }
    public bool IsDead { get; private set; }
    public string StatusMessage { get; private set; } = "The egg is warm and quiet...";

    public static Pet CreateNew(string name, PetSpecies species, Vector2 startPosition) => new(name, species, startPosition);

    public static Pet Restore(PetState state)
    {
        var pet = new Pet(state.Name, state.Species, state.Position)
        {
            Stage = state.Stage,
            AnimationState = state.AnimationState,
            IsSleeping = state.IsSleeping,
            IsDead = state.IsDead,
            AgeSeconds = state.AgeSeconds,
            StageTimeSeconds = state.StageTimeSeconds,
            ActionTimeRemaining = state.ActionTimeRemaining,
            StatusMessage = state.StatusMessage,
            Velocity = state.Velocity
        };

        pet.Needs.Set(state.Hunger, state.Happiness, state.Energy, state.Hygiene, state.Health);
        return pet;
    }

    public PetState ToState() => new(
        Name,
        Species,
        Stage,
        AnimationState,
        IsSleeping,
        IsDead,
        AgeSeconds,
        StageTimeSeconds,
        ActionTimeRemaining,
        StatusMessage,
        Position,
        Velocity,
        Needs.Hunger,
        Needs.Happiness,
        Needs.Energy,
        Needs.Hygiene,
        Needs.Health);

    public void AddAge(double deltaSeconds)
    {
        if (IsDead)
        {
            return;
        }

        AgeSeconds += deltaSeconds;
        StageTimeSeconds += deltaSeconds;
        ActionTimeRemaining = Math.Max(0, ActionTimeRemaining - deltaSeconds);

        if (ActionTimeRemaining <= 0 && !IsSleeping)
        {
            AnimationState = PetAnimationState.Idle;
        }
    }

    public void ChangeStage(PetStage stage, string message)
    {
        if (IsDead)
        {
            return;
        }

        Stage = stage;
        StageTimeSeconds = 0;
        SetStatusMessage(message);
    }

    public void SetSleeping(bool value)
    {
        if (IsDead)
        {
            return;
        }

        IsSleeping = value;
        AnimationState = value ? PetAnimationState.Sleeping : PetAnimationState.Idle;
        SetStatusMessage(value ? $"{Name} is sleeping..." : $"{Name} woke up feeling better.");
    }

    public void StartAction(PetAnimationState state, double durationSeconds, string message)
    {
        if (IsDead)
        {
            return;
        }

        if (IsSleeping)
        {
            SetSleeping(false);
        }

        AnimationState = state;
        ActionTimeRemaining = Math.Max(0, durationSeconds);
        SetStatusMessage(message);
    }

    public void MarkDead()
    {
        IsDead = true;
        IsSleeping = false;
        AnimationState = PetAnimationState.Idle;
        SetStatusMessage($"{Name} has died...");
    }

    public void SetPosition(Vector2 position) => Position = position;
    public void SetVelocity(Vector2 velocity) => Velocity = velocity;
    public void SetStatusMessage(string message) => StatusMessage = message;
}
