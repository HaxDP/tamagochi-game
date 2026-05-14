using System.Text;
using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Presentation.UI;

namespace ConsoleTamagotchi.Presentation.Rendering;

public sealed class ConsoleRenderer
{
    private readonly AsciiSpriteProvider _spriteProvider;

    public ConsoleRenderer(AsciiSpriteProvider spriteProvider)
    {
        _spriteProvider = spriteProvider;
    }

    public void Render(RenderModel model)
    {
        var width = Math.Max(60, Console.WindowWidth);
        var height = Math.Max(24, Console.WindowHeight);
        var worldWidth = Math.Clamp(model.WorldWidth, 30, width - 2);
        var worldHeight = Math.Clamp(model.WorldHeight, 10, height - 12);
        var lines = new string[height];

        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = new string(' ', width);
        }

        DrawWorldFrame(lines, width, worldWidth, worldHeight);
        DrawWorldEffects(lines, model.World, worldWidth, worldHeight);
        DrawPet(lines, model.Pet, model.ElapsedSeconds, worldWidth, worldHeight);
        DrawHud(lines, model, worldHeight, width);
        Flush(lines);
    }

    private static void DrawWorldFrame(string[] lines, int width, int worldWidth, int worldHeight)
    {
        WriteLine(lines, 0, 0, "+" + new string('-', worldWidth) + "+");
        for (var y = 1; y <= worldHeight; y++)
        {
            WriteLine(lines, y, 0, "|" + new string(' ', worldWidth) + "|");
        }

        WriteLine(lines, worldHeight + 1, 0, "+" + new string('-', worldWidth) + "+");

        if (width > worldWidth + 2)
        {
            for (var y = 0; y <= worldHeight + 1; y++)
            {
                WriteText(lines, y, worldWidth + 2, " ");
            }
        }
    }

    private void DrawPet(string[] lines, Pet pet, double elapsedSeconds, int worldWidth, int worldHeight)
    {
        var sprite = _spriteProvider.GetSprite(pet, elapsedSeconds);
        var x = (int)Math.Round(pet.Position.X) + 1;
        var y = (int)Math.Round(pet.Position.Y) + 1;
        var maxX = worldWidth - sprite.Width + 1;
        var maxY = worldHeight - sprite.Height + 1;
        x = Math.Clamp(x, 1, Math.Max(1, maxX));
        y = Math.Clamp(y, 1, Math.Max(1, maxY));

        for (var i = 0; i < sprite.Lines.Length; i++)
        {
            WriteText(lines, y + i, x, sprite.Lines[i]);
        }
    }

    private static void DrawWorldEffects(string[] lines, GameWorld world, int worldWidth, int worldHeight)
    {
        foreach (var poop in world.Poops)
        {
            var x = (int)Math.Round(poop.X) + 1;
            var y = (int)Math.Round(poop.Y) + 1;
            x = Math.Clamp(x, 1, worldWidth);
            y = Math.Clamp(y, 1, worldHeight);
            WriteText(lines, y, x, "@");
        }

        foreach (var particle in world.Particles)
        {
            var x = (int)Math.Round(particle.Position.X) + 1;
            var y = (int)Math.Round(particle.Position.Y) + 1;
            x = Math.Clamp(x, 1, worldWidth);
            y = Math.Clamp(y, 1, worldHeight);
            WriteText(lines, y, x, particle.Symbol.ToString());
        }
    }

    private static void DrawHud(string[] lines, RenderModel model, int worldHeight, int width)
    {
        var hudStart = worldHeight + 3;
        var pet = model.Pet;

        WriteLine(lines, hudStart, 0, $"Name: {pet.Name} | Species: {pet.Species} | Stage: {pet.Stage} | Age: {(int)pet.AgeSeconds}s");
        WriteLine(lines, hudStart + 1, 0, BuildNeedLine("Hunger", pet.Needs.Hunger, "Happiness", pet.Needs.Happiness));
        WriteLine(lines, hudStart + 2, 0, BuildNeedLine("Energy", pet.Needs.Energy, "Hygiene", pet.Needs.Hygiene));
        WriteLine(lines, hudStart + 3, 0, BuildSingleNeedLine("Health", pet.Needs.Health));
        WriteLine(lines, hudStart + 4, 0, $"Status: {pet.StatusMessage}");

        WriteLine(lines, hudStart + 6, 0, "Action Menu (Up/Down + Enter, hotkeys F/P/S/H, Left=Sleep, Right=Heal):");

        for (var i = 0; i < model.Actions.Count; i++)
        {
            var marker = model.SelectedActionIndex == i ? ">" : " ";
            var actionName = model.Actions[i].ToString().PadRight(5);
            WriteLine(lines, hudStart + 7 + i, 0, $"{marker} {i + 1}. {actionName}");
        }

        var footerY = Math.Min(lines.Length - 1, hudStart + 13);
        WriteLine(lines, footerY, 0, "Left click poop with mouse cursor to clean | Q or Esc: Quit");

        // Keep all rows fully padded to avoid rendering leftovers after resize.
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length < width)
            {
                lines[i] = lines[i].PadRight(width);
            }
        }
    }

    private static string BuildNeedLine(string leftLabel, double leftValue, string rightLabel, double rightValue)
    {
        var leftBar = BuildBar(leftValue);
        var rightBar = BuildBar(rightValue);
        return $"{leftLabel,-9}: {leftBar} {leftValue,5:0}   {rightLabel,-9}: {rightBar} {rightValue,5:0}";
    }

    private static string BuildSingleNeedLine(string label, double value)
    {
        var bar = BuildBar(value);
        return $"{label,-9}: {bar} {value,5:0}";
    }

    private static string BuildBar(double value)
    {
        const int width = 16;
        var filled = (int)Math.Round((value / 100d) * width);
        filled = Math.Clamp(filled, 0, width);
        return "[" + new string('#', filled) + new string('-', width - filled) + "]";
    }

    private static void WriteLine(string[] lines, int row, int col, string text)
    {
        WriteText(lines, row, col, text);
    }

    private static void WriteText(string[] lines, int row, int col, string text)
    {
        if (row < 0 || row >= lines.Length || string.IsNullOrEmpty(text))
        {
            return;
        }

        var line = lines[row].ToCharArray();
        for (var i = 0; i < text.Length; i++)
        {
            var x = col + i;
            if (x < 0 || x >= line.Length)
            {
                continue;
            }

            line[x] = text[i];
        }

        lines[row] = new string(line);
    }

    private static void Flush(string[] lines)
    {
        var buffer = new StringBuilder(lines.Length * (lines[0].Length + 1));
        for (var i = 0; i < lines.Length; i++)
        {
            buffer.Append(lines[i]);
            if (i < lines.Length - 1)
            {
                buffer.Append('\n');
            }
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(buffer.ToString());
    }
}
