using System;
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

    private void Validate()
    {
        if (Rank < 1 || File < 1 || Rank > 8 || File > 8)
        {
            throw new ArgumentOutOfRangeException(
                $"The square with the rank {Rank} and file {File} is not positioned on the 8x8 chess board");
        }
    }

    public string FindInMask(string[] mask)
    {
        Validate();
        return mask[FileIndex][RankIndex].ToString();
    }

    public T FindInMatrix<T>(T[,] matrix)
    {
        Validate();
        return matrix[RankIndex, FileIndex];
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
        return Of(Board.Ranks.IndexOf(notation[0]) + 1, Board.Files.IndexOf(notation[1]) + 1);
    }

    public static SquareLocation Of(int rank, int file)
    {
        return new SquareLocation { Rank = rank, File = file };
    }
}
