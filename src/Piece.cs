using System;
using System.Collections.Generic;
using Godot;

namespace GodotChess;

public partial class Piece : Area2D
{
    [Export] private Texture2D _whiteTexture;
    [Export] private Texture2D _blackTexture;

    private Sprite2D _sprite;
    private Side _side;
    private Game _game;
    private Node2D _hints;
    private PackedScene _hintPrefab;

    protected Board Board;
    public SquareLocation Location { get; set; }
    public Type PieceType { get; set; }
    public bool HasMoved { get; set; }

    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Queen,
        Rook,
        King
    }

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        Board = GetNode<Board>("/root/Game/Board");
        _game = GetNode<Game>("/root/Game");
        _hints = GetNode<Node2D>("/root/Game/Board/Hints");
        _hintPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/move_hint.tscn");
        
        InputEvent += OnClick;
    }

    protected virtual HashSet<SquareLocation> GenerateMoveLocations()
    {
        return new HashSet<SquareLocation>();
    }

    protected void AddIfCapturable(SquareLocation location, ref HashSet<SquareLocation> locations)
    {
        if (Board.GetSquare(location).IsOccupied) // TODO: deny capture if king will be checked
        {
            locations.Add(location);
        }
    }

    protected void AddIfNotCapturable(SquareLocation location, ref HashSet<SquareLocation> locations)
    {
        if (!Board.GetSquare(location).IsOccupied) // TODO: deny capture if king will be checked
        {
            locations.Add(location);
        }
    }
    
    protected static void AddLocation(ref HashSet<SquareLocation> locations, SquareLocation location)
    {
        locations.Add(location);
    }

    protected SquareLocation GetDeltaLocation(SquareLocation delta)
    {
        return Location + delta * Game.SideMultiplier;
    }
    
    protected void WithDeltaLocation(SquareLocation delta, Action<SquareLocation> action)
    {
        var result = GetDeltaLocation(delta);

        if (SquareLocation.IsValid(result))
        {
            action.Invoke(result);
        }
    }
    
    public void ColorAs(Side side)
    {
        _sprite.Texture = side == Side.White ? _whiteTexture : _blackTexture;
        _side = side;
    }

    public void DeleteHints()
    {
        foreach (var hint in _hints.GetChildren())
        {
            hint.QueueFree();
        }
    }

    private void OnClick(Node _, InputEvent input, long _1)
    {
        if (!input.IsPressed() || Game.SideMoving != _side) return;

        GD.Print("press");
        
        DeleteHints();
        var locations = GenerateMoveLocations();

        foreach (var location in locations)
        {
            var hint = _hintPrefab.Instantiate<MoveHint>();
            var move = ConvertLocationToMove(location);
            hint.HintedMove = move;
            hint.HintedPiece = this;
            hint.Position = move.TargetLocation.AsRelativePosition();
            _hints.AddChild(hint);
        }
    }

    private Move ConvertLocationToMove(SquareLocation targetLocation)
    {
        var isCapture = Board.GetSquare(targetLocation).IsOccupied;
        // TODO: account for checks, mates and promotions

        return new Move
            { Type = PieceType, SourceLocation = Location, TargetLocation = targetLocation, IsCapture = isCapture };
    }

    public static string EncodeTypeToNotation(Type type)
    {
        return type switch
        {
            Type.Pawn => null,
            Type.Knight => "N",
            Type.Bishop => "B",
            Type.Queen => "Q",
            Type.Rook => "R",
            Type.King => "K",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Type DecodeTypeFromNotation(string notation)
    {
        return notation switch
        {
            null => Type.Pawn,
            "P" => Type.Pawn, // alternative for pawns used in Board._pieceSetupMask
            "N" => Type.Knight,
            "B" => Type.Bishop,
            "Q" => Type.Queen,
            "R" => Type.Rook,
            "K" => Type.King,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
