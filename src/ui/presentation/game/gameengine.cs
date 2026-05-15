using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Application.Models;
using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Infrastructure.Abstractions;
using ConsoleTamagotchi.Infrastructure.Console;
using ConsoleTamagotchi.Presentation.Input;
using ConsoleTamagotchi.Presentation.Rendering;
using ConsoleTamagotchi.Presentation.UI;

namespace ConsoleTamagotchi.Presentation.Game;

public sealed class GameEngine
{
    private readonly Pet _pet;
    private readonly GameWorld _world;
    private readonly GameSettings _settings;
    private readonly IPetSimulationService _simulationService;
    private readonly IPetInteractionService _interactionService;
    private readonly IGameStateStore _stateStore;
    private readonly ConsoleRenderer _renderer;
    private readonly TerminalCursor _cursor;
    private readonly ConsoleKeyboardInput _inputReader;
    private readonly ConsoleMouseInput _mouseInput;
    private readonly ActionMenuCursor _menuCursor;
    private readonly IGameClock _clock;
    private readonly TimeSpan _tick = TimeSpan.FromMilliseconds(100);
    private readonly TimeSpan _autosaveInterval = TimeSpan.FromSeconds(1.5);
    private bool _isRunning = true;
    private DateTimeOffset _lastFrameAt;
    private DateTimeOffset _startedAt;
    private double _elapsedSeconds;
    private double _saveCountdownSeconds;

    public GameEngine(
        Pet pet,
        GameWorld world,
        GameSettings settings,
        IPetSimulationService simulationService,
        IPetInteractionService interactionService,
        IGameStateStore stateStore,
        ConsoleRenderer renderer,
        TerminalCursor cursor,
        ConsoleKeyboardInput inputReader,
        ConsoleMouseInput mouseInput,
        ActionMenuCursor menuCursor,
        IGameClock clock,
        double elapsedSeconds)
    {
        _pet = pet;
        _world = world;
        _settings = settings;
        _simulationService = simulationService;
        _interactionService = interactionService;
        _stateStore = stateStore;
        _renderer = renderer;
        _cursor = cursor;
        _inputReader = inputReader;
        _mouseInput = mouseInput;
        _menuCursor = menuCursor;
        _clock = clock;
        _elapsedSeconds = elapsedSeconds;
    }

    public GameRunResult Run()
    {
        Console.Clear();
        _cursor.Hide();
        _startedAt = _clock.UtcNow;
        _lastFrameAt = _startedAt;
        _saveCountdownSeconds = _autosaveInterval.TotalSeconds;

        while (_isRunning)
        {
            var now = _clock.UtcNow;
            var deltaSeconds = Math.Max(0.01, (now - _lastFrameAt).TotalSeconds);
            _lastFrameAt = now;
            _elapsedSeconds += deltaSeconds;

            var worldWidth = Math.Max(30, Console.WindowWidth - 2);
            var worldHeight = Math.Max(10, Console.WindowHeight - 14);
            ProcessInput(worldWidth, worldHeight);
            _simulationService.Update(_pet, _world, _settings, deltaSeconds, worldWidth, worldHeight);

            if (_pet.IsDead)
            {
                _stateStore.Delete();
                break;
            }

            _renderer.Render(new RenderModel(
                _pet,
                _world,
                _menuCursor.Actions,
                _menuCursor.SelectedIndex,
                worldWidth,
                worldHeight,
                _elapsedSeconds));

            _saveCountdownSeconds -= deltaSeconds;
            if (_saveCountdownSeconds <= 0)
            {
                _stateStore.Save(new GameState(_pet.ToState(), _world.ToState(), _settings, _elapsedSeconds));
                _saveCountdownSeconds = _autosaveInterval.TotalSeconds;
            }

            Thread.Sleep(_tick);
        }

        if (!_pet.IsDead)
        {
            _stateStore.Save(new GameState(_pet.ToState(), _world.ToState(), _settings, _elapsedSeconds));
        }

        _cursor.MoveTo(0, Math.Max(0, Console.WindowHeight - 1));
        _cursor.Show();
        return _pet.IsDead ? GameRunResult.Dead : GameRunResult.Quit;
    }

    private void ProcessInput(int worldWidth, int worldHeight)
    {
        ProcessMouseInput(worldWidth, worldHeight);

        var commands = _inputReader.ReadAvailable();
        foreach (var command in commands)
        {
            switch (command)
            {
                case InputCommand.MoveUp:
                    _menuCursor.MoveUp();
                    break;
                case InputCommand.MoveDown:
                    _menuCursor.MoveDown();
                    break;
                case InputCommand.QuickSleep:
                    ExecuteAction(ActionType.Sleep);
                    break;
                case InputCommand.QuickHeal:
                    ExecuteAction(ActionType.Heal);
                    break;
                case InputCommand.Select:
                    ExecuteAction(_menuCursor.SelectedAction);
                    break;
                case InputCommand.Feed:
                    ExecuteAction(ActionType.Feed);
                    break;
                case InputCommand.Play:
                    ExecuteAction(ActionType.Play);
                    break;
                case InputCommand.Sleep:
                    ExecuteAction(ActionType.Sleep);
                    break;
                case InputCommand.Heal:
                    ExecuteAction(ActionType.Heal);
                    break;
                case InputCommand.Quit:
                    _isRunning = false;
                    break;
                case InputCommand.None:
                default:
                    break;
            }
        }
    }

    private void ProcessMouseInput(int worldWidth, int worldHeight)
    {
        var clicks = _mouseInput.ReadLeftClicks();
        foreach (var click in clicks)
        {
            var worldX = click.X - 1;
            var worldY = click.Y - 1;
            if (worldX < 1 || worldY < 1 || worldX > worldWidth || worldY > worldHeight)
            {
                continue;
            }

            if (_world.TryCleanPoopAt(worldX, worldY))
            {
                _pet.Needs.ChangeHygiene(10);
                _pet.Needs.ChangeHappiness(2);
                _pet.SetStatusMessage("You cleaned poop with the cursor.");
            }
        }
    }

    private void ExecuteAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.Feed:
                _interactionService.Feed(_pet, _world);
                break;
            case ActionType.Play:
                _interactionService.Play(_pet, _world);
                break;
            case ActionType.Sleep:
                _interactionService.Sleep(_pet, _world);
                break;
            case ActionType.Heal:
                _interactionService.Heal(_pet, _world);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown action");
        }
    }
}
