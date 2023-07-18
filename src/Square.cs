using Godot;

namespace GodotChess;

public record Square
{
    public Vector2 RelativePosition { get; init; }
    public bool IsOccupied { get; set; }
    public Piece OccupyingPiece { get; set; }
}
