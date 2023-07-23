namespace GodotChess.Pieces;

public partial class Rook : Piece
{
    public override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var delta in SquareLocation.PerpendicularDeltas)
        {
            AddDirectionUntilObstructed(delta, ref moves);
        }

        return moves;
    }
}