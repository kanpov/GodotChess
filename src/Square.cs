namespace GodotChess;

public record Square
{
    public SquareLocation Location { get; init; }
    public bool IsOccupied { get; set; }
    public Piece OccupyingPiece { get; set; }
}