using System;
using Godot;

namespace GodotChess;

public partial class Piece : Node2D
{
    [Export] private Texture2D _whiteTexture;
    [Export] private Texture2D _blackTexture;
    
    private Sprite2D _sprite;
    
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
    }

    public bool CanMove(Board board, int rank, int file)
    {
        return false;
    }

    public void ColorAs(Side side)
    {
        _sprite.Texture = side == Side.White ? _whiteTexture : _blackTexture;
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
