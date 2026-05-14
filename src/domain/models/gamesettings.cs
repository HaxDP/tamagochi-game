namespace ConsoleTamagotchi.Domain.Models;

public sealed record GameSettings(double GrowthRate, double StatDecayRate)
{
    public static GameSettings Default => new(1.0, 1.0);
}
