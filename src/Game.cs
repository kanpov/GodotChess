global using Godot;
global using System;
global using System.Linq;
global using System.Text;
global using System.Collections.Generic;

using GodotChess.Pieces;

namespace GodotChess;

public partial class Game : Node2D
{
    [Export] private float _moveHalfDelay;

    private Board _board;
    private Camera2D _camera;
    private Timer _gameEndTimer;

    public static Side SideMoving { get; private set; }
    public static int SideInversion = 1;
    public static bool CanMove { get; private set; }

    public override void _Ready()
    {
        _board = GetNode<Board>("Board");
        _camera = GetNode<Camera2D>("Camera2D");
        _gameEndTimer = GetNode<Timer>("GameEndTimer");

        SideMoving = Side.White;
        CanMove = true;
    }

    public void ConfirmMove(Move move)
    {
        _board.ApplyMove(move);
        GD.Print($"{SideMoving} played: {move.EncodeToNotation()}");

        var newRotation = SideMoving == Side.White ? Mathf.DegToRad(180f) : Mathf.DegToRad(0f);
        _board.Flip(newRotation);
        _camera.Rotation = newRotation;

        if (SideMoving == Side.White)
        {
            SideMoving = Side.Black;
            SideInversion = -1;
        }
        else
        {
            SideMoving = Side.White;
            SideInversion = 1;
        }

        if (!King.IsSideMated(_board, SideMoving))
            return;

        CanMove = false;
        _gameEndTimer.Start();
        _gameEndTimer.Timeout += () => GetTree().Quit();
    }
}