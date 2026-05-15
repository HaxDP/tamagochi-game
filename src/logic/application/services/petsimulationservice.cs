using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Domain.ValueObjects;
using ConsoleTamagotchi.Infrastructure.Abstractions;

namespace ConsoleTamagotchi.Application.Services;

public sealed class PetSimulationService : IPetSimulationService
{
    private readonly IRandomProvider _random;
    private double _directionChangeCooldown;

    public PetSimulationService(IRandomProvider random)
    {
        _random = random;
    }

    public void Update(Pet pet, GameWorld world, GameSettings settings, double deltaSeconds, int worldWidth, int worldHeight)
    {
        world.Tick(deltaSeconds);
        pet.AddAge(deltaSeconds);
        UpdateStage(pet, settings);
        ApplyNeedDecay(pet, settings, deltaSeconds);
        UpdateWorldEffects(pet, world, worldWidth, worldHeight);
        UpdateMotion(pet, deltaSeconds, worldWidth, worldHeight);
    }

    private static void UpdateStage(Pet pet, GameSettings settings)
    {
        var growthRate = Math.Max(0.25, settings.GrowthRate);
        if (pet.AgeSeconds >= 15 / growthRate && pet.Stage == PetStage.Egg)
        {
            pet.ChangeStage(PetStage.Baby, $"{pet.Name} hatched! Welcome to the world.");
            return;
        }

        if (pet.AgeSeconds >= 65 / growthRate && pet.Stage == PetStage.Baby)
        {
            pet.ChangeStage(PetStage.Teen, $"{pet.Name} grew into a teen.");
            return;
        }

        if (pet.AgeSeconds >= 130 / growthRate && pet.Stage == PetStage.Teen)
        {
            pet.ChangeStage(PetStage.Adult, $"{pet.Name} became a proud adult.");
        }
    }

    private static void ApplyNeedDecay(Pet pet, GameSettings settings, double deltaSeconds)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            return;
        }

        var decayRate = Math.Max(0.2, settings.StatDecayRate);
        var adjustedDelta = deltaSeconds * decayRate;

        if (pet.IsSleeping)
        {
            pet.Needs.ChangeEnergy(26 * adjustedDelta);
            pet.Needs.ChangeHunger(-2.2 * adjustedDelta);
            pet.Needs.ChangeHygiene(-0.7 * adjustedDelta);

            if (pet.Needs.Energy >= 98)
            {
                pet.SetSleeping(false);
            }
        }
        else
        {
            pet.Needs.ChangeHunger(-2.8 * adjustedDelta);
            pet.Needs.ChangeHappiness(-1.2 * adjustedDelta);
            pet.Needs.ChangeEnergy(-1.6 * adjustedDelta);
            pet.Needs.ChangeHygiene(-1.1 * adjustedDelta);
        }

        var criticalNeed = pet.Needs.Hunger < 20 || pet.Needs.Energy < 15 || pet.Needs.Hygiene < 20;
        var severeNeed = pet.Needs.Hunger < 8 || pet.Needs.Energy < 5 || pet.Needs.Hygiene < 10;

        if (severeNeed)
        {
            pet.Needs.ChangeHealth(-8 * adjustedDelta);
        }
        else if (criticalNeed)
        {
            pet.Needs.ChangeHealth(-3.5 * adjustedDelta);
        }
        else if (pet.Needs.Health < 100)
        {
            pet.Needs.ChangeHealth(1.2 * adjustedDelta);
        }

        if (pet.Needs.Health <= 0)
        {
            pet.MarkDead();
        }
    }

    private void UpdateWorldEffects(Pet pet, GameWorld world, int worldWidth, int worldHeight)
    {
        if (pet.IsDead || pet.Stage == PetStage.Egg)
        {
            return;
        }

        if (pet.Needs.Hygiene < 60)
        {
            world.TrySpawnPoop(_random, worldWidth, worldHeight);
        }

        if (pet.Needs.Happiness >= 70 && !pet.IsSleeping)
        {
            world.TrySpawnHappyParticles(pet, _random);
        }
    }

    private void UpdateMotion(Pet pet, double deltaSeconds, int worldWidth, int worldHeight)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            var centerX = Math.Max(1, worldWidth / 2 - 3);
            var centerY = Math.Max(1, worldHeight / 2 - 2);
            pet.SetPosition(new Vector2(centerX, centerY));
            return;
        }

        if (pet.IsSleeping)
        {
            return;
        }

        _directionChangeCooldown -= deltaSeconds;
        if (_directionChangeCooldown <= 0)
        {
            _directionChangeCooldown = _random.NextDouble(0.8, 2.4);
            var steerX = _random.NextDouble(-0.8, 0.8);
            var steerY = _random.NextDouble(-0.7, 0.7);
            var nextVelocity = new Vector2(pet.Velocity.X + steerX, pet.Velocity.Y + steerY);
            pet.SetVelocity(EnsureMinSpeed(nextVelocity));
        }

        var speed = GetStageSpeed(pet.Stage);
        if (pet.Needs.Energy < 25)
        {
            speed *= 0.35;
        }

        var nextPosition = pet.Position + pet.Velocity * (speed * deltaSeconds);
        var spriteWidth = 9;
        var spriteHeight = 5;
        var minX = 1;
        var minY = 1;
        var maxX = Math.Max(minX, worldWidth - spriteWidth - 1);
        var maxY = Math.Max(minY, worldHeight - spriteHeight - 1);
        var velocity = pet.Velocity;

        if (nextPosition.X < minX || nextPosition.X > maxX)
        {
            velocity = new Vector2(-velocity.X, velocity.Y);
            nextPosition = new Vector2(Math.Clamp(nextPosition.X, minX, maxX), nextPosition.Y);
        }

        if (nextPosition.Y < minY || nextPosition.Y > maxY)
        {
            velocity = new Vector2(velocity.X, -velocity.Y);
            nextPosition = new Vector2(nextPosition.X, Math.Clamp(nextPosition.Y, minY, maxY));
        }

        pet.SetVelocity(EnsureMinSpeed(velocity));
        pet.SetPosition(nextPosition);
    }

    private static Vector2 EnsureMinSpeed(Vector2 velocity)
    {
        var x = Math.Abs(velocity.X) < 0.35 ? Math.Sign(velocity.X == 0 ? 1 : velocity.X) * 0.35 : velocity.X;
        var y = Math.Abs(velocity.Y) < 0.25 ? Math.Sign(velocity.Y == 0 ? 1 : velocity.Y) * 0.25 : velocity.Y;
        return new Vector2(x, y);
    }

    private static double GetStageSpeed(PetStage stage) => stage switch
    {
        PetStage.Baby => 6.5,
        PetStage.Teen => 8.2,
        PetStage.Adult => 7.5,
        _ => 0
    };
}
