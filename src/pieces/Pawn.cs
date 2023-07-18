using System.Collections.Generic;

namespace GodotChess.Pieces;

public partial class Pawn : Piece
{
    protected override HashSet<SquareLocation> GenerateMoveLocations()
    {
        var locations = new HashSet<SquareLocation>();
        
        WithDeltaLocation(SquareLocation.Deltas.North, location => AddIfNotCapturable(location, ref locations));
        WithDeltaLocation(SquareLocation.Deltas.North * 2, location =>
        {
            if (!HasMoved) AddIfNotCapturable(location, ref locations);
        });
        
        WithDeltaLocation(SquareLocation.Deltas.Northeast, location => AddIfCapturable(location, ref locations));
        WithDeltaLocation(SquareLocation.Deltas.Northwest, location => AddIfCapturable(location, ref locations));

        // TODO: add en passant
        
        return locations;
    }
}
