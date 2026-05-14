using ConsoleTamagotchi.Infrastructure.Abstractions;

namespace ConsoleTamagotchi.Infrastructure.Time;

public sealed class SystemGameClock : IGameClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
