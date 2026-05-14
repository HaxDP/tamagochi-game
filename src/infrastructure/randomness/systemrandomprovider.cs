using ConsoleTamagotchi.Infrastructure.Abstractions;

namespace ConsoleTamagotchi.Infrastructure.Randomness;

public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly Random _random = Random.Shared;

    public double NextDouble() => _random.NextDouble();

    public double NextDouble(double min, double max)
    {
        if (max < min)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "max must be >= min");
        }

        return min + (_random.NextDouble() * (max - min));
    }
}
