namespace ConsoleTamagotchi.Presentation.Rendering;

public sealed record AsciiSprite(string[] Lines)
{
    public int Width => Lines.Max(line => line.Length);
    public int Height => Lines.Length;
}
