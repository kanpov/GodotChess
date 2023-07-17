using System;
using Godot;

namespace GodotChess;

public partial class Board : Node2D
{
    public const string Ranks = "abcdefgh";
    public const string Files = "12345678";
    private const int SquarePixelSize = 128;
    
    private readonly string[] _squareTypeMask =
    {
        ".;.;.;.;",
        ";.;.;.;.",
        ".;.;.;.;",
        ";.;.;.;.",
        ".;.;.;.;",
        ";.;.;.;.",
        ".;.;.;.;",
        ";.;.;.;."
    };

    private readonly string[] _pieceSetupMask =
    {
        "RNBQKBNR",
        "PPPPPPPP",
        "........",
        "........",
        "........",
        "........",
        "PPPPPPPP",
        "RNBQKBNR"
    };

    [Export] public Texture2D LightSquare;
    [Export] public Texture2D DarkSquare;

    private PackedScene _squarePrefab; private PackedScene _pawnPrefab; private PackedScene _knightPrefab;
    private PackedScene _bishopPrefab; private PackedScene _rookPrefab; private PackedScene _queenPrefab;
    private PackedScene _kingPrefab;

    private Square[,] _squares;
    
    public override void _Ready()
    {
        _squarePrefab = ResourceLoader.Load<PackedScene>("res://prefabs/square.tscn");
        _pawnPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/pawn.tscn");
        _knightPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/knight.tscn");
        _bishopPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/bishop.tscn");
        _rookPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/rook.tscn");
        _queenPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/queen.tscn");
        _kingPrefab = ResourceLoader.Load<PackedScene>("res://prefabs/king.tscn");

        _squares = new Square[8, 8];
        Build();
        AddPieces();
    }

    private void Build()
    {
        for (var rank = 1; rank <= 8; ++rank)
        {
            for (var file = 1; file <= 8; ++file)
            {
                var square = _squarePrefab.Instantiate<Sprite2D>();
                square.Texture = _squareTypeMask[rank - 1][file - 1] == ';' ? DarkSquare : LightSquare;
                square.Position = new Vector2(SquarePixelSize * (rank - 1), SquarePixelSize * (8 - file));
                AddChild(square);

                _squares[rank - 1, file - 1] = new Square
                    { RelativePosition = square.Position, IsOccupied = false, OccupyingPiece = null };
            }
        }
    }

    private void AddPieces()
    {
        for (var rank = 1; rank <= 8; ++rank)
        {
            for (var file = 1; file <= 8; ++file)
            {
                var notation = _pieceSetupMask[rank - 1][file - 1].ToString();
                if (notation == ".") continue;
                
                var pieceType = Piece.DecodeTypeFromNotation(notation);
                var prefab = pieceType switch
                {
                    Piece.Type.Pawn => _pawnPrefab,
                    Piece.Type.Knight => _knightPrefab,
                    Piece.Type.Bishop => _bishopPrefab,
                    Piece.Type.Rook => _rookPrefab,
                    Piece.Type.Queen => _queenPrefab,
                    Piece.Type.King => _kingPrefab,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                AddPiece(prefab, SquareLocation.Of(rank, file));
            }
        }
    }

    private void AddPiece(PackedScene prefab, SquareLocation location)
    {
        var piece = prefab.Instantiate<Piece>();
        var square = GetSquare(location);
        square.IsOccupied = true;
        square.OccupyingPiece = piece;
        AddChild(piece);
        piece.Position = square.RelativePosition;
        piece.ColorAs(location.File < 5 ? Side.White : Side.Black);
    }

    public Square GetSquare(SquareLocation location)
    {
        location.Validate();
        return _squares[location.Rank - 1, location.File - 1];
    }

    public void ApplyMove(Move move)
    {
        var source = GetSquare(move.SourceLocation);
        var piece = source.OccupyingPiece;
        source.IsOccupied = false;
        source.OccupyingPiece = null;
        var target = GetSquare(move.TargetLocation);
        target.IsOccupied = true;
        target.OccupyingPiece = piece;
        piece.Position = target.RelativePosition;
    }
}
