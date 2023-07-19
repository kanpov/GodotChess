using System.Collections.Generic;
using System.Linq;

namespace GodotChess.Pieces;

public partial class King : Piece
{
    public override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var delta in SquareLocation.AllDeltas)
        {
            Add(GetDeltaLocation(delta), ref moves);
        }

        return moves;
    }

    public static bool IsChecked(Board board)
    {
        var result = false;
        
        SquareLocation.RunOnAll(location =>
        {
            var square = board.GetSquare(location);

            if (!square.IsOccupied) return;

            var moves = square.OccupyingPiece.GenerateMoves();

            result = moves.Select(move => board.GetSquare(move.Value))
                .Any(newSquare => newSquare.IsOccupied && newSquare.OccupyingPiece is King _);
        });

        return result;
    }
}
