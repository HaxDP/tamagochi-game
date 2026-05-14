using ConsoleTamagotchi.Application.Services;
using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Domain.ValueObjects;
using ConsoleTamagotchi.Infrastructure.Randomness;

namespace ConsoleTamagotchi.Tests;

public class UnitTest1
{
    [Fact]
    public void Feed_IncreasesHunger_ForBaby()
    {
        var pet = Pet.CreateNew("Mochi", PetSpecies.Cat, new Vector2(0, 0));
        var world = new GameWorld();
        pet.ChangeStage(PetStage.Baby, "ready");
        pet.Needs.ChangeHunger(-60);

        var sut = new PetInteractionService();
        sut.Feed(pet, world);

        Assert.True(pet.Needs.Hunger > 40);
        Assert.Equal(PetAnimationState.Eating, pet.AnimationState);
    }

    [Fact]
    public void Update_HatchesEgg_AfterEnoughTime()
    {
        var pet = Pet.CreateNew("Mochi", PetSpecies.Cat, new Vector2(5, 5));
        var world = new GameWorld();
        var settings = GameSettings.Default;
        var sut = new PetSimulationService(new SystemRandomProvider());

        sut.Update(pet, world, settings, 16, 80, 30);

        Assert.Equal(PetStage.Baby, pet.Stage);
    }

    [Fact]
    public void Update_MarksPetDead_WhenHealthDropsToZero()
    {
        var pet = Pet.CreateNew("Mochi", PetSpecies.Dog, new Vector2(5, 5));
        var world = new GameWorld();
        var settings = new GameSettings(1.0, 3.0);
        var sut = new PetSimulationService(new SystemRandomProvider());
        pet.ChangeStage(PetStage.Baby, "ready");
        pet.Needs.Set(0, 0, 0, 0, 1);

        sut.Update(pet, world, settings, 1.5, 80, 30);

        Assert.True(pet.IsDead);
    }
}