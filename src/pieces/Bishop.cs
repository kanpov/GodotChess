using System.Collections.Generic;

namespace GodotChess.Pieces;

public partial class Bishop : Piece
{
    public override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var delta in SquareLocation.DiagonalDeltas)
        {
            AddDirectionUntilObstructed(delta, ref moves);
        }
        
        return moves;
    }
}
