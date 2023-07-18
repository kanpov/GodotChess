using System;
using System.Collections.Generic;
using Godot;

namespace GodotChess;

public record SquareLocation
{
    public int Rank { get; private init; }
    public int File { get; private init; }

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
        return Create(Board.Ranks.IndexOf(notation[0]) + 1, Board.Files.IndexOf(notation[1]) + 1);
    }

    public static SquareLocation operator +(SquareLocation a, SquareLocation b)
    {
        return Create(a.Rank + b.Rank, a.File + b.File);
    }

    public static SquareLocation operator *(SquareLocation a, int b)
    {
        return Create(a.Rank * b, a.File * b);
    }
    
    public static SquareLocation Create(int rank, int file)
    {
        return new SquareLocation { Rank = rank, File = file };
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
                action.Invoke(Create(rank, file));
            }
        }
    }

    // Simple deltas
    public static readonly SquareLocation North = Create(0, 1);
    public static readonly SquareLocation South = Create(0, -1);
    public static readonly SquareLocation East = Create(1, 0);
    public static readonly SquareLocation West = Create(-1, 0);
    public static readonly SquareLocation Northeast = Create(1, 1);
    public static readonly SquareLocation Northwest = Create(-1, 1);
    public static readonly SquareLocation Southeast = Create(1, -1);
    public static readonly SquareLocation Southwest = Create(-1, -1);
    public static readonly List<SquareLocation> Diagonals = new() { Northeast, Northwest, Southeast, Southwest };
}
