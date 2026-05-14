namespace ConsoleTamagotchi.Infrastructure.Abstractions;

public interface IGameClock
{
    DateTimeOffset UtcNow { get; }
}
