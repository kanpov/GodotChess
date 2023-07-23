namespace GodotChess;

public partial class MoveHint : Area2D
{
    public Move HintedMove { get; set; }
    public Piece HintedPiece { get; set; }

    private Game _game;

    public override void _Ready()
    {
        _game = GetNode<Game>("/root/Game");
        InputEvent += OnClicked;
    }

    private void OnClicked(Node _, InputEvent input, long _1)
    {
        if (!input.IsPressed())
            return;

        _game.ConfirmMove(HintedMove);
        HintedPiece.DeleteHints();
    }
}