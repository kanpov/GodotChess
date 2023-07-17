using System.Linq;
using System.Text;

namespace GodotChess;

public record Move
{
    public Piece.Type PieceType { get; init; }
    public SquareLocation From { get; init; }
    public SquareLocation To { get; init; }
    public bool IsCapture { get; init; }
    public bool IsCheck { get; init; }
    public bool IsMate { get; init; }
    public bool IsPromotion { get; init; }
    public Piece.Type PromotedPieceType { get; init; }

    public string EncodeToNotation()
    {
        var builder = new StringBuilder();
        
        var pieceTypeStr = Piece.EncodeTypeToNotation(PieceType);
        if (pieceTypeStr != null) builder.Append(pieceTypeStr);

        builder.Append(From.EncodeToNotation());
        if (IsCapture) builder.Append('x');
        builder.Append(To.EncodeToNotation());

        if (IsPromotion)
        {
            builder.Append('=');
            builder.Append(Piece.EncodeTypeToNotation(PromotedPieceType));
        }

        if (IsCheck) builder.Append('+');
        if (IsMate) builder.Append('#');

        return builder.ToString();
    }

    public static Move DecodeFromNotation(string notation)
    {
        var pieceType = Piece.DecodeTypeFromNotation(notation[0].ToString());
        var from = SquareLocation.DecodeFromNotation(notation.Substring(1, 2));
        var isCapture = notation[3] == 'x';
        var to = isCapture
            ? SquareLocation.DecodeFromNotation(notation.Substring(4, 2))
            : SquareLocation.DecodeFromNotation(notation.Substring(3, 2));
        var isCheck = notation.EndsWith('+');
        var isMate = notation.EndsWith('#');
        var isPromotion = notation.Contains('=');

        var promotedPieceType = Piece.Type.Pawn;
        if (isPromotion)
        {
            foreach (var c in notation.Where(c => char.IsUpper(c) && notation.IndexOf(c) > 0))
            {
                promotedPieceType = Piece.DecodeTypeFromNotation(c.ToString());
            }
        }

        return new Move
        {
            PieceType = pieceType, From = from, To = to, IsCapture = isCapture, IsCheck = isCheck, IsMate = isMate,
            IsPromotion = isPromotion, PromotedPieceType = promotedPieceType
        };
    }
}