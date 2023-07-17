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

    [Export] public Texture2D LightSquare;
    [Export] public Texture2D DarkSquare;

    private PackedScene _squarePrefab;

    private Square[,] _squares;
    
    public override void _Ready()
    {
        _squarePrefab = ResourceLoader.Load<PackedScene>("res://prefabs/square.tscn");
        _squares = new Square[8, 8];
        Build();
    }

    private void Build()
    {
        for (var rank = 1; rank <= 8; ++rank)
        {
            for (var file = 1; file <= 8; ++file)
            {
                var square = _squarePrefab.Instantiate<Sprite2D>();
                square.Texture = _squareTypeMask[rank - 1][file - 1] == ';' ? DarkSquare : LightSquare;
                square.Position = new Vector2(SquarePixelSize * (rank - 1), SquarePixelSize * (file - 1));
                AddChild(square);

                _squares[rank - 1, file - 1] = new Square
                    { RelativePosition = square.Position, IsOccupied = false, OccupyingPiece = null };
            }
        }
    }

    public Square GetSquare(SquareLocation location)
    {
        location.Validate();
        return _squares[location.Rank - 1, location.File - 1];
    }
}
