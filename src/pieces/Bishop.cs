using System.Collections.Generic;

namespace GodotChess.Pieces;

public partial class Bishop : Piece
{
    protected override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var diagonal in SquareLocation.Diagonals)
        {
            AddMovesInDiagonal(diagonal, ref moves);
        }
        
        return moves;
    }

    private void AddMovesInDiagonal(SquareLocation diagonal, ref HashSet<MoveContext> moves)
    {
        var multiplier = 0;
        
        while (true)
        {
            ++multiplier;
            var location = GetDeltaLocation(diagonal * multiplier);

            if (Add(location, ref moves) || Board.GetSquare(location).IsOccupied) break;
        }
    }
}
