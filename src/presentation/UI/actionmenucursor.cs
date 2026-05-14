namespace ConsoleTamagotchi.Presentation.UI;

public sealed class ActionMenuCursor
{
    private readonly ActionType[] _actions =
    {
        ActionType.Feed,
        ActionType.Play,
        ActionType.Sleep,
        ActionType.Heal
    };

    public int SelectedIndex { get; private set; }
    public IReadOnlyList<ActionType> Actions => _actions;
    public ActionType SelectedAction => _actions[SelectedIndex];

    public void MoveUp()
    {
        SelectedIndex = (SelectedIndex - 1 + _actions.Length) % _actions.Length;
    }

    public void MoveDown()
    {
        SelectedIndex = (SelectedIndex + 1) % _actions.Length;
    }
}
