using System;
using Godot;

namespace GodotChess;

public partial class Piece : Node2D
{
    private Sprite2D _sprite;
    
    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Queen,
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

    public void Setup(Texture2D texture)
    {
        _sprite.Texture = texture;
    }

    public static string EncodeTypeToNotation(Type type)
    {
        return type switch
        {
            Type.Pawn => null,
            Type.Knight => "N",
            Type.Bishop => "B",
            Type.Queen => "Q",
            Type.King => "K",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static Type DecodeTypeFromNotation(string notation)
    {
        return notation switch
        {
            null => Type.Pawn,
            "N" => Type.Knight,
            "B" => Type.Bishop,
            "Q" => Type.Queen,
            "K" => Type.King,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
