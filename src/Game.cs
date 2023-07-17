using Godot;

namespace GodotChess;

public partial class Game : Node2D
{
    [Export] private float _moveDelay;
    
    private Board _board;
    private Camera2D _camera;
    private bool _canMove;
    private Side _sideMoving;

    public override void _Ready()
    {
        _board = GetNode<Board>("Board");
        _camera = GetNode<Camera2D>("Camera2D");
        _sideMoving = Side.White;
        _canMove = true;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("confirm_move") && _canMove)
        {
            ConfirmMove();
        }
    }

    private void ConfirmMove()
    {
        _canMove = false;
        var tween = GetTree().CreateTween();
        var newRotation = _sideMoving == Side.White ? Mathf.DegToRad(180f) : Mathf.DegToRad(0f);
        tween.TweenProperty(_camera, "rotation", newRotation, _moveDelay);
        tween.Finished += () =>
        {
            _canMove = true;
            _sideMoving = _sideMoving == Side.White ? Side.Black : Side.White;
        };
    }
}
