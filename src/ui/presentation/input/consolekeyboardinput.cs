namespace ConsoleTamagotchi.Presentation.Input;

public sealed class ConsoleKeyboardInput
{
    public List<InputCommand> ReadAvailable()
    {
        var commands = new List<InputCommand>();
        while (System.Console.KeyAvailable)
        {
            var key = System.Console.ReadKey(intercept: true).Key;
            commands.Add(Map(key));
        }

        return commands;
    }

    private static InputCommand Map(ConsoleKey key) => key switch
    {
        ConsoleKey.UpArrow => InputCommand.MoveUp,
        ConsoleKey.DownArrow => InputCommand.MoveDown,
        ConsoleKey.LeftArrow => InputCommand.QuickSleep,
        ConsoleKey.RightArrow => InputCommand.QuickHeal,
        ConsoleKey.Enter => InputCommand.Select,
        ConsoleKey.F => InputCommand.Feed,
        ConsoleKey.P => InputCommand.Play,
        ConsoleKey.S => InputCommand.Sleep,
        ConsoleKey.H => InputCommand.Heal,
        ConsoleKey.Q or ConsoleKey.Escape => InputCommand.Quit,
        _ => InputCommand.None
    };
}
