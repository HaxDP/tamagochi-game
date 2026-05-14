namespace ConsoleTamagotchi.Infrastructure.Console;

public sealed class TerminalCursor
{
    public void Hide() => System.Console.CursorVisible = false;
    public void Show() => System.Console.CursorVisible = true;

    public void MoveTo(int x, int y)
    {
        var safeX = Math.Clamp(x, 0, Math.Max(0, System.Console.WindowWidth - 1));
        var safeY = Math.Clamp(y, 0, Math.Max(0, System.Console.WindowHeight - 1));
        System.Console.SetCursorPosition(safeX, safeY);
    }
}
