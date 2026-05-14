using ConsoleTamagotchi.Domain.Models;
using ConsoleTamagotchi.Presentation.UI;

namespace ConsoleTamagotchi.Presentation.Rendering;

public sealed record RenderModel(
    Pet Pet,
    GameWorld World,
    IReadOnlyList<ActionType> Actions,
    int SelectedActionIndex,
    int WorldWidth,
    int WorldHeight,
    double ElapsedSeconds);
