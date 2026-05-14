namespace ConsoleTamagotchi.Domain.Models;

public sealed class PetNeeds
{
    public double Hunger { get; private set; } = 75;
    public double Happiness { get; private set; } = 80;
    public double Energy { get; private set; } = 80;
    public double Hygiene { get; private set; } = 80;
    public double Health { get; private set; } = 100;

    public void Set(double hunger, double happiness, double energy, double hygiene, double health)
    {
        Hunger = Clamp(hunger);
        Happiness = Clamp(happiness);
        Energy = Clamp(energy);
        Hygiene = Clamp(hygiene);
        Health = Clamp(health);
    }

    public void ChangeHunger(double delta) => Hunger = Clamp(Hunger + delta);
    public void ChangeHappiness(double delta) => Happiness = Clamp(Happiness + delta);
    public void ChangeEnergy(double delta) => Energy = Clamp(Energy + delta);
    public void ChangeHygiene(double delta) => Hygiene = Clamp(Hygiene + delta);
    public void ChangeHealth(double delta) => Health = Clamp(Health + delta);

    private static double Clamp(double value) => Math.Clamp(value, 0, 100);
}
