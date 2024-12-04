using System.Text;
using System.Text.RegularExpressions;

namespace Day04;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        string[] inputLines = ReadInput(inputPath);
        int xmasCount = CountAllWordSearch(inputLines, "XMAS");
        Console.WriteLine($"Total XMAS count: {xmasCount}");
        int maxCrossCount = CountAllMasCrosses(inputLines);
        Console.WriteLine($"Total X-MAS count: {maxCrossCount}");
    }

    private static string[] ReadInput(string filePath)
    {
        return File.ReadAllLines(filePath);
    }
    
    private static int CountAllWordSearch(string[] inputLines, string target)
    {
        return CountHorizontal(inputLines, target) + CountVertical(inputLines, target) +
               CountDiagonal(inputLines, target);
    }

    private static int CountHorizontal(string[] inputLines, string target)
    {
        int width = inputLines[0].Length;
        if (inputLines.Any(s => s.Length != width))
        {
            throw new ArgumentException("Not a rectangular 2d array");
        }
        
        return inputLines.Select(s => CountInString(s, target)).Sum();
    }
    
    private static int CountVertical(string[] inputLines, string target)
    {
        int width = inputLines[0].Length;
        if (inputLines.Any(s => s.Length != width))
        {
            throw new ArgumentException("Not a rectangular 2d array");
        }
        
        int count = 0;
        
        for (int i = 0; i < width; i++)
        {
            string vertical = String.Join("", inputLines.Select(s => s[i]));
            count += CountInString(vertical, target);
        }

        return count;
    }
    
    private static int CountDiagonal(string[] inputLines, string target)
    {
        int height = inputLines.Length;
        int width = inputLines[0].Length;
        if (inputLines.Any(s => s.Length != width))
        {
            throw new ArgumentException("Not a rectangular 2d array");
        }
        
        int count = 0;
        
        // top-left to bottom-right, along left edge
        for (int i = 0; i < height; i++)
        {
            StringBuilder diagonalBuilder = new StringBuilder();
            for (int j = 0; j < Math.Min(height - i, width); j++)
            {
                diagonalBuilder.Append(inputLines[i + j][j]);
            }
            count += CountInString(diagonalBuilder.ToString(), target);
        }
        
        // top-left to bottom-right, along top edge
        // skip corner
        for (int i = 1; i < width; i++)
        {
            StringBuilder diagonalBuilder = new StringBuilder();
            for (int j = 0; j < Math.Min(height, width - i); j++)
            {
                diagonalBuilder.Append(inputLines[j][i + j]);
            }
            count += CountInString(diagonalBuilder.ToString(), target);
        }
        
        // top-right to bottom-left, along right edge
        //skip corner
        for (int i = 1; i < height; i++)
        {
            StringBuilder diagonalBuilder = new StringBuilder();
            for (int j = 0; j < Math.Min(height - i, width); j++)
            {
                diagonalBuilder.Append(inputLines[i + j][width - (j + 1)]);
            }
            count += CountInString(diagonalBuilder.ToString(), target);
        }
        
        // top-right to bottom-left, along top edge
        for (int i = 0; i < width; i++)
        {
            StringBuilder diagonalBuilder = new StringBuilder();
            for (int j = 0; j < Math.Min(height, i + 1); j++)
            {
                diagonalBuilder.Append(inputLines[j][i - j]);
            }
            count += CountInString(diagonalBuilder.ToString(), target);
        }
        
        return count;
    }

    private static int CountInString(string stringToSearch, string target)
    {
        int forwardsCount = Regex.Count(stringToSearch, target);
        char[] array = stringToSearch.ToCharArray();
        Array.Reverse(array);
        string backwards = new string(array);
        int backwardsCount = Regex.Count(backwards, target);
        return forwardsCount + backwardsCount;
    }

    private static int CountAllMasCrosses(string[] inputLines)
    {
        if (inputLines.Any(s => s.Length != inputLines[0].Length))
        {
            throw new ArgumentException("Not a rectangular 2d array");
        }
        
        int count = 0;
        
        // Skip first and last rows/columns since need to check all adjacent diagonals
        for (int i = 1; i < inputLines.Length - 1; i++)
        {
            for (int j = 1; j < inputLines[0].Length - 1; j++)
            {
                if (IsMasCross(inputLines, i, j)) count++;
            }
        }

        return count;
    }

    private static bool IsMasCross(string[] inputLines, int i, int j)
    {
        if (i < 1 || i > inputLines.Length - 2 || j < 1 || j > inputLines[0].Length - 2)
        {
            throw new ArgumentException("Index cannot be on border");
        }

        if (inputLines[i][j] != 'A') return false;

        char tl = inputLines[i - 1][j - 1];
        char tr = inputLines[i - 1][j + 1];
        char bl = inputLines[i + 1][j - 1];
        char br = inputLines[i + 1][j + 1];
        List<char> corners = [tl, tr, bl, br];

        if (corners.Any(c => c != 'M' && c != 'S')) return false;

        return tl != br && tr != bl;
    }
}