using System.Collections.Generic;

namespace GodotChess.Pieces;

public partial class Pawn : Piece
{
    protected override HashSet<MoveContext> GenerateMoveLocations()
    {
        var locations = new HashSet<MoveContext>();
        
        // 1-square forward movement
        WithDeltaLocation(SquareLocation.Deltas.North, location => AddIfNotCapturable(location, ref locations));
        // 2-square forward movement (one-time)
        WithDeltaLocation(SquareLocation.Deltas.North * 2, location =>
        {
            if (!HasMoved) AddIfNotCapturable(location, ref locations);
        });
        
        // diagonal capture
        WithDeltaLocation(SquareLocation.Deltas.Northeast, location => AddIfCapturable(location, ref locations));
        WithDeltaLocation(SquareLocation.Deltas.Northwest, location => AddIfCapturable(location, ref locations));

        // en passant capture
        AddEnPassant(SquareLocation.Deltas.East, SquareLocation.Deltas.Northeast, ref locations);
        AddEnPassant(SquareLocation.Deltas.West, SquareLocation.Deltas.Northwest, ref locations);
        
        return locations;
    }

    private void AddEnPassant(SquareLocation horizontalDelta, SquareLocation diagonalDelta, 
        ref HashSet<MoveContext> locations)
    {
        // movement does not go outside the board
        if (SquareLocation.IsInvalid(GetDeltaLocation(horizontalDelta)) ||
            SquareLocation.IsInvalid(GetDeltaLocation(diagonalDelta))) return;
        
        // targeted at a pawn on files 4 or 5 that has moved once
        var horizontalSquare = Board.GetSquare(GetDeltaLocation(horizontalDelta));

        if (!horizontalSquare.IsOccupied || horizontalSquare.OccupyingPiece is not Pawn otherPawn) return;
        if ((otherPawn.Location.File != 4 && otherPawn.Location.File != 5) || otherPawn.MoveAmount > 1) return;

        locations.Add(new MoveContext(GetDeltaLocation(diagonalDelta), IsEnPassant: true,
            EnPassantLocation: GetDeltaLocation(horizontalDelta)));
    }
}
