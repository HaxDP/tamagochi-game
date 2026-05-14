using ConsoleTamagotchi.Application.Interfaces;
using ConsoleTamagotchi.Application.Services;
using ConsoleTamagotchi.Infrastructure.Console;
using ConsoleTamagotchi.Infrastructure.Persistence;
using ConsoleTamagotchi.Infrastructure.Randomness;
using ConsoleTamagotchi.Infrastructure.Time;
using ConsoleTamagotchi.Presentation.Game;
using ConsoleTamagotchi.Presentation.Input;
using ConsoleTamagotchi.Presentation.Rendering;
using ConsoleTamagotchi.Presentation.Startup;
using ConsoleTamagotchi.Presentation.UI;

var randomProvider = new SystemRandomProvider();
var clock = new SystemGameClock();
var simulationService = new PetSimulationService(randomProvider);
IPetInteractionService interactionService = new PetInteractionService();
var stateStore = new JsonGameStateStore(Path.Combine(AppContext.BaseDirectory, "savegame.json"));
var spriteProvider = new AsciiSpriteProvider();
var renderer = new ConsoleRenderer(spriteProvider);
var cursor = new TerminalCursor();
var inputReader = new ConsoleKeyboardInput();
using var mouseInput = new ConsoleMouseInput();
var menuCursor = new ActionMenuCursor();
var startupMenu = new StartupMenu(stateStore);

while (true)
{
    var startSpec = startupMenu.Show();
    var engine = new GameEngine(
        startSpec.Pet,
        startSpec.World,
        startSpec.Settings,
        simulationService,
        interactionService,
        stateStore,
        renderer,
        cursor,
        inputReader,
        mouseInput,
        menuCursor,
        clock,
        startSpec.ElapsedSeconds);

    var result = engine.Run();
    if (result == GameRunResult.Quit)
    {
        break;
    }

    Console.Clear();
    Console.CursorVisible = true;
    Console.WriteLine("Your pet died. Starting over...");
    Thread.Sleep(1400);
}
