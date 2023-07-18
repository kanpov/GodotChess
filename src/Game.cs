using Godot;

namespace GodotChess;

public partial class Game : Node2D
{
    [Export] private float _moveHalfDelay;

    private Board _board;
    private Camera2D _camera;

    public static Side SideMoving { get; private set; }
    public static int SideMultiplier = 1;

    public override void _Ready()
    {
        _board = GetNode<Board>("Board");
        _camera = GetNode<Camera2D>("Camera2D");
        
        SideMoving = Side.White;
    }

    public void ConfirmMove(Move move)
    {
        _board.ApplyMove(move);
        
        var newRotation = SideMoving == Side.White ? Mathf.DegToRad(180f) : Mathf.DegToRad(0f);
        _board.Flip(newRotation);
        _camera.Rotation = newRotation;

        if (SideMoving == Side.White)
        {
            SideMoving = Side.Black;
            SideMultiplier = -1;
        }
        else
        {
            SideMoving = Side.White;
            SideMultiplier = 1;
        }
    }
}
