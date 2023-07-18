using System.Collections.Generic;
using System.Linq;

namespace GodotChess.Pieces;

public partial class Knight : Piece
{
    private static readonly List<SquareLocation> KnightDeltas = new() {
        SquareLocation.Create(2, 1), SquareLocation.Create(2, -1), SquareLocation.Create(-2, 1),
        SquareLocation.Create(-2, -1), SquareLocation.Create(1, 2), SquareLocation.Create(-1, 2),
        SquareLocation.Create(1, -2), SquareLocation.Create(-1, -2)
    };
    
    protected override HashSet<MoveContext> GenerateMoves()
    {
        var moves = new HashSet<MoveContext>();

        foreach (var location in KnightDeltas.Select(GetDeltaLocation).Where(SquareLocation.IsValid))
        {
            Add(location, ref moves);
        }
        
        return moves;
    }
}
