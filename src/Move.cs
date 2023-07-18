using System.Linq;
using System.Text;
using Godot;

namespace GodotChess;

public partial class Move : GodotObject
{
    public Piece.Type Type { get; init; }
    public SquareLocation SourceLocation { get; init; }
    public SquareLocation TargetLocation { get; init; }
    public bool IsCapture { get; init; }
    public bool IsCheck { get; init; }
    public bool IsMate { get; init; }
    public bool IsPromotion { get; init; }
    public Piece.Type PromotedType { get; init; }

    public string EncodeToNotation()
    {
        var builder = new StringBuilder();
        
        var pieceTypeStr = Piece.EncodeTypeToNotation(Type);
        if (pieceTypeStr != null) builder.Append(pieceTypeStr);

        builder.Append(SourceLocation.EncodeToNotation());
        if (IsCapture) builder.Append('x');
        builder.Append(TargetLocation.EncodeToNotation());

        if (IsPromotion)
        {
            builder.Append('=');
            builder.Append(Piece.EncodeTypeToNotation(PromotedType));
        }

        if (IsCheck) builder.Append('+');
        if (IsMate) builder.Append('#');

        return builder.ToString();
    }

    public static Move DecodeFromNotation(string notation)
    {
        var pieceType = Piece.DecodeTypeFromNotation(notation[0].ToString());
        var sourceLocation = SquareLocation.DecodeFromNotation(notation.Substring(1, 2));
        var isCapture = notation[3] == 'x';
        var targetLocation = isCapture
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
            Type = pieceType, SourceLocation = sourceLocation, TargetLocation = targetLocation, IsCapture = isCapture, IsCheck = isCheck, IsMate = isMate,
            IsPromotion = isPromotion, PromotedType = promotedPieceType
        };
    }
}