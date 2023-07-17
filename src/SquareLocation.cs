using System;

namespace GodotChess;

public record SquareLocation
{
    public int Rank { get; init; }
    public int File { get; init; }

    public void Validate()
    {
        if (Rank < 1 || File < 1 || Rank > 8 || File > 8)
        {
            throw new ArgumentOutOfRangeException(
                $"The square with the rank {Rank} and file {File} is not positioned on the 8x8 chess board");
        }
    }
    
    public string EncodeToNotation()
    {
        return Board.Ranks[Rank - 1] + Board.Files[File - 1].ToString();
    }

    public static SquareLocation DecodeFromNotation(string notation)
    {
        return new SquareLocation
        {
            Rank = Board.Ranks.IndexOf(notation[0]) + 1,
            File = Board.Files.IndexOf(notation[1]) + 1
        };
    }

    public static SquareLocation Of(int rank, int file)
    {
        return new SquareLocation { Rank = rank, File = file };
    }
}
