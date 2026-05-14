namespace ConsoleTamagotchi.Infrastructure.Abstractions;

public interface IRandomProvider
{
    double NextDouble();
    double NextDouble(double min, double max);
}
