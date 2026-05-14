using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Domain.Enums;
using ConsoleTamagotchi.Domain.Models;

namespace ConsoleTamagotchi.Application.Services;

public sealed class PetInteractionService : IPetInteractionService
{
    public void Feed(Pet pet, GameWorld world)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            pet.SetStatusMessage("The egg cannot eat yet.");
            return;
        }

        pet.Needs.ChangeHunger(28);
        pet.Needs.ChangeHappiness(4);
        pet.Needs.ChangeEnergy(2);
        pet.StartAction(PetAnimationState.Eating, 2.2, $"{pet.Name} enjoyed a tasty snack.");
    }

    public void Play(Pet pet, GameWorld world)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            pet.SetStatusMessage("The egg wiggles softly when you play music.");
            return;
        }

        pet.Needs.ChangeHappiness(20);
        pet.Needs.ChangeEnergy(-12);
        pet.Needs.ChangeHunger(-6);
        pet.Needs.ChangeHygiene(-8);
        pet.StartAction(PetAnimationState.Idle, 1.8, $"{pet.Name} had a lot of fun.");
    }

    public void Clean(Pet pet, GameWorld world)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            pet.SetStatusMessage("You gently polished the egg shell.");
            return;
        }

        pet.Needs.ChangeHygiene(35);
        pet.Needs.ChangeHappiness(2);
        world.ClearPoops();
        pet.SetStatusMessage($"{pet.Name} is now fresh and clean.");
    }

    public void Sleep(Pet pet, GameWorld world)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            pet.SetStatusMessage("The egg is already resting.");
            return;
        }

        pet.SetSleeping(!pet.IsSleeping);
    }

    public void Heal(Pet pet, GameWorld world)
    {
        if (pet.IsDead)
        {
            return;
        }

        if (pet.Stage == PetStage.Egg)
        {
            pet.SetStatusMessage("The egg does not need medicine.");
            return;
        }

        pet.Needs.ChangeHealth(30);
        pet.Needs.ChangeHappiness(-3);
        pet.SetStatusMessage($"{pet.Name} took medicine and feels stronger.");
    }
}
