namespace GodotChess.Pieces;

public partial class Knight : Piece
{
    private static readonly List<SquareLocation> KnightDeltas = new() {
        new SquareLocation(2, 1), new SquareLocation(2, -1), new SquareLocation(-2, 1),
        new SquareLocation(-2, -1), new SquareLocation(1, 2), new SquareLocation(-1, 2),
        new SquareLocation(1, -2), new SquareLocation(-1, -2)
    };

    public override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var location in KnightDeltas.Select(GetDeltaLocation).Where(SquareLocation.IsValid))
        {
            Add(location, ref moves);
        }

        return moves;
    }
}