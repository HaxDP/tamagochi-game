using ConsoleTamagotchi.Domain.ValueObjects;
using ConsoleTamagotchi.Infrastructure.Abstractions;

namespace ConsoleTamagotchi.Domain.Models;

public sealed class GameWorld
{
    private readonly List<Vector2> _poops;
    private readonly List<WorldParticle> _particles;

    public GameWorld()
    {
        _poops = [];
        _particles = [];
    }

    private GameWorld(GameWorldState state)
    {
        _poops = state.Poops ?? [];
        _particles = state.Particles ?? [];
        NextPoopSpawnIn = state.NextPoopSpawnIn;
        NextParticleSpawnIn = state.NextParticleSpawnIn;
    }

    public IReadOnlyList<Vector2> Poops => _poops;
    public IReadOnlyList<WorldParticle> Particles => _particles;
    public double NextPoopSpawnIn { get; private set; }
    public double NextParticleSpawnIn { get; private set; }

    public static GameWorld Restore(GameWorldState state) => new(state);

    public GameWorldState ToState() => new([.. _poops], [.. _particles], NextPoopSpawnIn, NextParticleSpawnIn);

    public void Tick(double deltaSeconds)
    {
        NextPoopSpawnIn = Math.Max(0, NextPoopSpawnIn - deltaSeconds);
        NextParticleSpawnIn = Math.Max(0, NextParticleSpawnIn - deltaSeconds);

        if (_particles.Count == 0)
        {
            return;
        }

        var updated = new List<WorldParticle>(_particles.Count);
        foreach (var particle in _particles)
        {
            var nextTtl = particle.TimeToLive - deltaSeconds;
            if (nextTtl > 0)
            {
                updated.Add(particle with { TimeToLive = nextTtl });
            }
        }

        _particles.Clear();
        _particles.AddRange(updated);
    }

    public void TrySpawnPoop(IRandomProvider random, int worldWidth, int worldHeight)
    {
        if (NextPoopSpawnIn > 0 || _poops.Count >= 18)
        {
            return;
        }

        NextPoopSpawnIn = random.NextDouble(0.7, 2.2);
        var x = random.NextDouble(1, Math.Max(2, worldWidth - 2));
        var y = random.NextDouble(1, Math.Max(2, worldHeight - 2));
        _poops.Add(new Vector2(x, y));
    }

    public void TrySpawnHappyParticles(Pet pet, IRandomProvider random)
    {
        if (NextParticleSpawnIn > 0)
        {
            return;
        }

        var count = pet.Needs.Happiness >= 90 ? 4 : 2;
        NextParticleSpawnIn = random.NextDouble(0.2, 0.45);
        for (var i = 0; i < count; i++)
        {
            var offsetX = random.NextDouble(-2.4, 2.4);
            var offsetY = random.NextDouble(-1.5, 1.5);
            var symbols = new[] { '*', '+', '.', 'o' };
            var symbol = symbols[(int)Math.Floor(random.NextDouble(0, symbols.Length)) % symbols.Length];
            _particles.Add(new WorldParticle(
                new Vector2(pet.Position.X + offsetX, pet.Position.Y + offsetY),
                random.NextDouble(0.4, 1.2),
                symbol));
        }
    }

    public void ClearPoops()
    {
        _poops.Clear();
    }

    public bool TryCleanPoopAt(int worldX, int worldY)
    {
        for (var i = 0; i < _poops.Count; i++)
        {
            var poop = _poops[i];
            if (Math.Abs(Math.Round(poop.X) - worldX) <= 1 && Math.Abs(Math.Round(poop.Y) - worldY) <= 1)
            {
                _poops.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
