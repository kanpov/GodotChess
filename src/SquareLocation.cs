using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotChess;

public record SquareLocation(int Rank, int File)
{
    public int Rank { get; } = Rank;
    public int File { get; } = File;

    private const int SquarePixelSize = 128;
    private int RankIndex => Rank - 1;
    private int FileIndex => File - 1;
    private int FileIndexInWorld => 8 - File;

    public string FindInMask(string[] mask)
    {
        if (IsInvalid(this)) throw new ArgumentOutOfRangeException();
        return mask[FileIndex][RankIndex].ToString();
    }

    public ref T FindInMatrix<T>(T[,] matrix)
    {
        if (IsInvalid(this)) throw new ArgumentOutOfRangeException();
        return ref matrix[RankIndex, FileIndex];
    }

    public Vector2 AsRelativePosition()
    {
        return new Vector2(SquarePixelSize * RankIndex, SquarePixelSize * FileIndexInWorld);
    }

    public string EncodeToNotation()
    {
        return Board.Ranks[Rank - 1] + Board.Files[File - 1].ToString();
    }

    public static SquareLocation DecodeFromNotation(string notation)
    {
        return new SquareLocation(Board.Ranks.IndexOf(notation[0]) + 1, Board.Files.IndexOf(notation[1]) + 1);
    }

    public static SquareLocation operator +(SquareLocation a, SquareLocation b)
    {
        return new SquareLocation(a.Rank + b.Rank, a.File + b.File);
    }

    public static SquareLocation operator *(SquareLocation a, int b)
    {
        return new SquareLocation(a.Rank * b, a.File * b);
    }
    
    public static bool IsInvalid(SquareLocation location)
    {
        return location.Rank < 1 || location.Rank > 8 || location.File < 1 || location.File > 8;
    }

    public static bool IsValid(SquareLocation location)
    {
        return !IsInvalid(location);
    }

    public static void RunOnAll(Action<SquareLocation> action)
    {
        for (var rank = 1; rank <= 8; ++rank)
        {
            for (var file = 1; file <= 8; ++file)
            {
                action.Invoke(new SquareLocation(rank, file));
            }
        }
    }

    public static readonly SquareLocation North = new(0, 1);
    public static readonly SquareLocation South = new(0, -1);
    public static readonly SquareLocation East = new(1, 0);
    public static readonly SquareLocation West = new(-1, 0);
    public static readonly SquareLocation Northeast = new(1, 1);
    public static readonly SquareLocation Northwest = new(-1, 1);
    public static readonly SquareLocation Southeast = new(1, -1);
    public static readonly SquareLocation Southwest = new(-1, -1);
    public static readonly List<SquareLocation> DiagonalDeltas = new() { Northeast, Northwest, Southeast, Southwest };
    public static readonly List<SquareLocation> PerpendicularDeltas = new() { North, South, East, West };
    public static readonly List<SquareLocation> AllDeltas = DiagonalDeltas.Concat(PerpendicularDeltas).ToList();
}
