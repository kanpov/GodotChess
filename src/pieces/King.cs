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

    // Static check & mate utilities
    public static bool IsSideCheckedAfterMove(Move move, Board board, Side side)
    {
        var reversalContext = board.MockMove(move);

        if (IsSideChecked(board, side))
        {
            board.RevertMockedMove(move, reversalContext);
            return true;
        }

        board.RevertMockedMove(move, reversalContext);
        return false;
    }

    public static bool IsSideMatedAfterMove(Move move, Board board, Side side)
    {
        var reversalContext = board.MockMove(move);

        if (IsSideMated(board, side))
        {
            board.RevertMockedMove(move, reversalContext);
            return true;
        }

        board.RevertMockedMove(move, reversalContext);
        return false;
    }

    public static bool IsSideChecked(Board board, Side side)
    {
        var result = false;

        SquareLocation.RunOnAll(location =>
        {
            var square = board.GetSquare(location);

            if (!square.IsOccupied || square.OccupyingPiece.Side == side)
                return;

            var moves = square.OccupyingPiece.GenerateMoves();

            if (!result)
                result = moves.Select(move => board.GetSquare(move.Value))
                .Any(newSquare => newSquare.IsOccupied && newSquare.OccupyingPiece is King king && king.Side == side);
        });

        return result;
    }

    public static bool IsSideMated(Board board, Side side)
    {
        for (var rank = 1; rank <= 8; ++rank)
        {
            for (var file = 1; file <= 8; ++file)
            {
                var square = board.GetSquare(new SquareLocation(rank, file));

                if (!square.IsOccupied || square.OccupyingPiece is not { } piece || piece.Side != side)
                    continue;

                if (piece.GenerateMoves()
                    .Select(context => ConvertContextToMove(context, board, piece.PieceType, piece.Location))
                    .Any(move => !IsSideCheckedAfterMove(move, board, side)))
                {
                    return false;
                }
            }
        }

        return true;
    }
}