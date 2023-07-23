namespace GodotChess.Pieces;

public partial class Queen : Piece
{
    public override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var delta in SquareLocation.AllDeltas)
        {
            AddDirectionUntilObstructed(delta, ref moves);
        }

        return moves;
    }
}