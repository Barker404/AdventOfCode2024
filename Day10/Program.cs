using Exception = System.Exception;

namespace Day10;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        int[][] topoGrid = ReadInput(inputPath);

        int trailScoreSum = GetTrailScoreSum(topoGrid);
        Console.WriteLine($"Sum of trailhead scores: {trailScoreSum}");

        int[][] trailGrid = GetZeroGrid(topoGrid.Length, topoGrid[0].Length);
        FillTrailGrid(topoGrid, trailGrid);
        int trailRatingSum = GetTrailRatingSum(topoGrid, trailGrid);
        Console.WriteLine($"Sum of trailhead ratings: {trailRatingSum}");
    }

    private static int[][] ReadInput(string inputPath)
    {
        int[][] topoGrid = File.ReadLines(inputPath).Select(
            line => line.Select(c => int.Parse(c.ToString())).ToArray()
        ).ToArray();

        if (topoGrid.Length == 0)
        {
            throw new Exception("Empty input");
        }

        if (topoGrid.Any(row => row.Length != topoGrid[0].Length))
        {
            throw new Exception("Not a rectangular grid");
        }

        return topoGrid;
    }

    private static IEnumerable<(int i, int j)> GetReachableNines(int[][] topoGrid, int i, int j)
    {
        if (topoGrid[i][j] == 9)
        {
            yield return (i, j);
        }
        else
        {
            foreach ((int i, int j) adjacent in GetAdjacent(topoGrid, i, j))
            {
                if (topoGrid[adjacent.i][adjacent.j] == topoGrid[i][j] + 1)
                {
                    foreach ((int i, int j) reachable in GetReachableNines(topoGrid, adjacent.i, adjacent.j))
                    {
                        yield return reachable;
                    }
                }
            }
        }
    }

    private static int GetTrailScoreSum(int[][] topoGrid)
    {
        return topoGrid.Select(
            (row, i) => row.Select(
                (height, j) => (height, i, j)
            ).Where(
                tuple => tuple.height == 0
            ).Select(
                tuple => GetReachableNines(topoGrid, tuple.i, tuple.j).ToHashSet().Count
            )
        ).SelectMany(x => x).Sum();
    }

    private static int[][] GetZeroGrid(int height, int width)
    {
        return Enumerable.Range(0, height).Select(
            _ => Enumerable.Repeat(0, width).ToArray()
        ).ToArray();
    }

    private static void FillTrailGrid(int[][] topoGrid, int[][] trailGrid)
    {
        for (int i = 0; i < topoGrid.Length; i++)
        {
            for (int j = 0; j < topoGrid[i].Length; j++)
            {
                if (topoGrid[i][j] == 9)
                {
                    FollowTrail(topoGrid, trailGrid, i, j);
                }
            }
        }
    }

    private static void FollowTrail(int[][] topoGrid, int[][] trailGrid, int i, int j)
    {
        trailGrid[i][j]++;
        foreach ((int i, int j) adjacent in GetAdjacent(trailGrid, i, j))
        {
            if (topoGrid[adjacent.i][adjacent.j] == topoGrid[i][j] - 1)
            {
                FollowTrail(topoGrid, trailGrid, adjacent.i, adjacent.j);
            }
        }
    }

    private static IEnumerable<(int i, int j)> GetAdjacent(int[][] grid, int i, int j)
    {
        if (i + 1 < grid.Length)
        {
            yield return (i + 1, j);
        }

        if (i - 1 >= 0)
        {
            yield return (i - 1, j);
        }

        if (j + 1 < grid[i].Length)
        {
            yield return (i, j + 1);
        }

        if (j - 1 >= 0)
        {
            yield return (i, j - 1);
        }
    }

    private static int GetTrailRatingSum(int[][] topoGrid, int[][] trailGrid)
    {
        return topoGrid.Select(
            (row, i) => row.Select(
                (height, j) => (height, i, j)
            ).Where(
                tuple => tuple.height == 0
            ).Select(
                tuple => trailGrid[tuple.i][tuple.j]
            )
        ).SelectMany(x => x).Sum();
    }
}