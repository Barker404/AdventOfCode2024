using System.Text.RegularExpressions;

namespace Day14;

class Program
{
    private static readonly Regex LineRegex = new(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)");

    private static readonly Coordinate AreaSize = new(101, 103);

    enum Quadrant
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4,
    }

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        List<(Coordinate p, Coordinate v)> robots = ReadInput(inputPath);

        List<Coordinate> finalPositions = robots.Select(
            robot => GetPositionAfter(robot.p, robot.v, 100, AreaSize)
        ).ToList();

        int safetyFactor = GetSafetyFactor(finalPositions, AreaSize);
        Console.WriteLine($"Safety factor after 100 seconds: {safetyFactor}");

        List<Func<Dictionary<Coordinate, int>, Coordinate, bool>> heuristics =
        [
            IsVerticallySymmetrical,
            (robotMap, areaSize) => HasXInARow(robotMap, areaSize, 8),
            (robotMap, areaSize) => MoreThanXRatioHasAdjacent(robotMap, areaSize, 0.6),
        ];

        List<(int, Dictionary<Coordinate, int>)> candidateOutputs = Enumerable.Range(1, 10000).Select(
            i => (i, GetPositionMap(robots.Select(
                robot => GetPositionAfter(robot.p, robot.v, i, AreaSize)
            )))
        ).Where(
            pair => heuristics.Any(
                heuristic => heuristic(pair.Item2, AreaSize)
            )
        ).ToList();

        foreach ((int i, Dictionary<Coordinate, int> robotMap) in candidateOutputs)
        {
            DisplayCandidate(robotMap, AreaSize, i);
        }

        Console.WriteLine($"Total candidates: {candidateOutputs.Count}");
    }

    private static List<(Coordinate p, Coordinate v)> ReadInput(string inputPath)
    {
        return File.ReadLines(inputPath).Select(ParseRobot).ToList();
    }

    private static (Coordinate p, Coordinate v) ParseRobot(string line)
    {
        Match match = LineRegex.Match(line);
        if (!match.Success) throw new ArgumentException("Line did not match expected pattern");
        return (
            new Coordinate(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
            new Coordinate(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
        );
    }

    private static Coordinate GetPositionAfter(Coordinate startPosition, Coordinate velocity, int steps,
        Coordinate areaSize)
    {
        // Double modulo to ensure negative velocity results in positive position 
        Coordinate modulo = (startPosition + velocity * steps) % areaSize;
        return (modulo + areaSize) % areaSize;
    }

    private static Dictionary<Coordinate, int> GetPositionMap(IEnumerable<Coordinate> positions)
    {
        return positions.GroupBy(
            c => c
        ).ToDictionary(
            group => group.Key, group => group.Count()
        );
    }

    private static int GetSafetyFactor(List<Coordinate> finalPositions, Coordinate areaSize)
    {
        return finalPositions.Select(
            coordinate => GetQuadrant(coordinate, areaSize)
        ).Where(
            quadrant => quadrant != Quadrant.None
        ).GroupBy(
            quadrant => quadrant
        ).Select(
            group => group.Count()
        ).Aggregate(1, (i, j) => i * j);
    }

    private static Quadrant GetQuadrant(Coordinate coordinate, Coordinate areaSize)
    {
        if ((areaSize.X % 2 == 1 && coordinate.X == areaSize.X / 2) ||
            (areaSize.Y % 2 == 1 && coordinate.Y == areaSize.Y / 2))
        {
            return Quadrant.None;
        }

        bool right = coordinate.X >= (areaSize.X + 1) / 2;
        bool bottom = coordinate.Y >= (areaSize.Y + 1) / 2;

        return (Quadrant)((right ? 1 : 0) + (bottom ? 2 : 0) + 1);
    }

    private static bool IsVerticallySymmetrical(Dictionary<Coordinate, int> robotMap, Coordinate areaSize)
    {
        return robotMap.All(
            entry => robotMap.ContainsKey(GetVerticalMirror(entry.Key, areaSize))
        );
    }

    private static Coordinate GetVerticalMirror(Coordinate coordinate, Coordinate areaSize)
    {
        return coordinate with { X = areaSize.X - 1 - coordinate.X };
    }

    private static bool ContainsAllColumns(Dictionary<Coordinate, int> robotMap, Coordinate areaSize)
    {
        // Not a good heuristic, matches large proportion of maps
        return robotMap.Select(entry => entry.Key.X).Distinct().Count() == areaSize.X;
    }

    private static bool HasXInARow(Dictionary<Coordinate, int> robotMap, Coordinate _, int x)
    {
        Coordinate hStep = new Coordinate(1, 0);
        Coordinate vStep = new Coordinate(0, 1);
        return robotMap.Any(
            entry => Enumerable.Range(1, x - 1).All(
                i => robotMap.ContainsKey(entry.Key + hStep * i)
            ) || Enumerable.Range(1, x).All(
                i => robotMap.ContainsKey(entry.Key + vStep * i)
            )
        );
    }

    private static bool MoreThanXRatioHasAdjacent(Dictionary<Coordinate, int> robotMap, Coordinate _, double x)
    {
        Coordinate left = new Coordinate(-1, 0);
        Coordinate right = new Coordinate(1, 0);
        Coordinate up = new Coordinate(0, -1);
        Coordinate down = new Coordinate(0, 1);
        int adjacentCount = robotMap.Count(
            entry => robotMap.ContainsKey(entry.Key + left) ||
                     robotMap.ContainsKey(entry.Key + right) ||
                     robotMap.ContainsKey(entry.Key + up) ||
                     robotMap.ContainsKey(entry.Key + down)
        );

        return adjacentCount > robotMap.Count * x;
    }

    private static void DisplayCandidate(Dictionary<Coordinate, int> robotMap, Coordinate areaSize, int step)
    {
        Console.WriteLine($"Candidate at step {step}:");
        foreach (int y in Enumerable.Range(0, areaSize.Y))
        {
            Console.WriteLine(string.Join("",
                Enumerable.Range(0, areaSize.X).Select(
                    x => robotMap.ContainsKey(new Coordinate(x, y)) ? robotMap[new Coordinate(x, y)].ToString() : "."
                )
            ));
        }
    }
}