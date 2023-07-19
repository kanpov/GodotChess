using System;
using Godot;

namespace GodotChess;

public partial class Board : Node2D
{
    public const string Ranks = "abcdefgh";
    public const string Files = "12345678";

    // Read-only masks are indexed by file first, and then by rank!
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
        SquareLocation.RunOnAll(location =>
        {
            var square = _squarePrefab.Instantiate<Sprite2D>();
            square.Texture = location.FindInMask(_squareTypeMask) == ";" ? LightSquare : DarkSquare;
            square.Position = location.AsRelativePosition();
            AddChild(square);

            location.FindInMatrix(_squares) = new Square
                { Location = location, IsOccupied = false, OccupyingPiece = null };
        });
    }

    private void AddPieces()
    {
        SquareLocation.RunOnAll(location =>
        {
            var notation = location.FindInMask(_pieceSetupMask);
            if (notation == ".") return;
            
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
                
            AddPiece(prefab, location, pieceType);
        });
    }

    private void AddPiece(PackedScene prefab, SquareLocation location, Piece.Type pieceType)
    {
        var piece = prefab.Instantiate<Piece>();
        var square = GetSquare(location);
        square.IsOccupied = true;
        square.OccupyingPiece = piece;
        AddChild(piece);
        piece.Position = square.Location.AsRelativePosition();
        piece.ColorAs(location.File < 5 ? Side.White : Side.Black);
        piece.Location = location;
        piece.PieceType = pieceType;
    }

    public Square GetSquare(SquareLocation location)
    {
        return location.FindInMatrix(_squares);
    }

    public void ApplyMove(Move move)
    {
        var source = GetSquare(move.SourceLocation);
        var piece = source.OccupyingPiece;
        var target = GetSquare(move.TargetLocation);
        
        if (move.IsCapture)
        {
            target.OccupyingPiece.QueueFree();
        }

        if (move.IsEnPassant)
        {
            GetSquare(move.EnPassantLocation).OccupyingPiece.QueueFree();
        }
        
        piece.Position = target.Location.AsRelativePosition();
        
        MockMove(move);
    }

    public Move.ReversalContext MockMove(Move move)
    {
        var source = GetSquare(move.SourceLocation);
        var piece = source.OccupyingPiece;
        var target = GetSquare(move.TargetLocation);

        Piece enPassantPiece = null;
        if (move.IsEnPassant)
        {
            var enPassantSquare = GetSquare(move.EnPassantLocation);
            enPassantSquare.IsOccupied = false;
            enPassantPiece = enPassantSquare.OccupyingPiece;
            enPassantSquare.OccupyingPiece = null;
        }
        
        source.IsOccupied = false;
        source.OccupyingPiece = null;
        var capturedPiece = target.OccupyingPiece;
        target.IsOccupied = true;
        target.OccupyingPiece = piece;
        piece.Location = move.TargetLocation;
        var hadPieceBeenMoved = piece.HasMoved;
        piece.HasMoved = true;
        piece.MoveAmount++;

        return new Move.ReversalContext(hadPieceBeenMoved, enPassantPiece, move.IsCapture ? capturedPiece : null);
    }

    public void RevertMockedMove(Move move, Move.ReversalContext reversalContext)
    {
        var source = GetSquare(move.SourceLocation);
        var target = GetSquare(move.TargetLocation);

        source.IsOccupied = true;
        source.OccupyingPiece = target.OccupyingPiece;
        source.OccupyingPiece.Location = move.SourceLocation;

        if (move.IsCapture)
        {
            target.IsOccupied = true;
            target.OccupyingPiece = reversalContext.CapturedPiece;
        }
        else
        {
            target.IsOccupied = false;
            target.OccupyingPiece = null;
        }

        if (move.IsEnPassant)
        {
            var enPassantSquare = GetSquare(move.EnPassantLocation);
            enPassantSquare.IsOccupied = true;
            enPassantSquare.OccupyingPiece = reversalContext.EnPassantPiece;
        }
        
        source.OccupyingPiece.MoveAmount--;
        if (!reversalContext.HadPieceBeenMoved) source.OccupyingPiece.HasMoved = false;
    }

    public void Flip(float newRotation)
    {
        SquareLocation.RunOnAll(location =>
        {
            var square = GetSquare(location);
            if (square.IsOccupied)
            {
                square.OccupyingPiece.Rotation = newRotation;
            }
        });
    }
}
