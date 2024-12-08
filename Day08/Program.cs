namespace Day08;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        string[] lines = File.ReadAllLines(inputPath);
        Dictionary<char, List<Coordinate>> antennaPositions = GetAntennaPositions(lines);
        Coordinate gridSize = GetGridSize(lines);

        HashSet<Coordinate> antinodes = GetAntinodes(antennaPositions, gridSize, false);
        Console.WriteLine($"Number of unique antinodes: {antinodes.Count}");
        
        HashSet<Coordinate> resonantAntinodes = GetAntinodes(antennaPositions, gridSize, true);
        Console.WriteLine($"Number of unique resonant antinodes: {resonantAntinodes.Count}");
    }

    private static Dictionary<char, List<Coordinate>> GetAntennaPositions(string[] lines)
        => lines.Select(
            (line, y) => line.Select((c, x) => (c, x, y)).Where(entry => entry.c != '.')
        ).SelectMany(
            i => i
        ).GroupBy(
            entry => entry.c, entry => (entry.x, entry.y)
        ).ToDictionary(
            group => group.Key,
            group => group.Select(entry => new Coordinate(entry.x, entry.y)).ToList()
        );
    
    private static Coordinate GetGridSize(string[] lines)
    {
        if (lines.Length == 0)
        {
            return new Coordinate(0, 0);
        }

        int width = lines[0].Length;
        if (lines.Any(line => line.Length != width))
        {
            throw new ArgumentException("Grid not square");
        }
        
        return new Coordinate(width, lines.Length);
    }
    
    private static HashSet<Coordinate> GetAntinodes(Dictionary<char, List<Coordinate>> antennaPositions, Coordinate gridSize, bool useResonance)
        => antennaPositions.Select(
            entry => entry.Value.Select(
                coord1 => entry.Value.Where(coord2 => coord1 != coord2)
                    .Select(
                        coord2 => Enumerable.Range(
                            useResonance ? 0 : 1, useResonance ? Math.Min(gridSize.X, gridSize.Y) : 1
                        ).Select(
                            i => coord2 + (coord2 - coord1) * i
                        ).TakeWhile(antinode => IsWithinBounds(antinode, gridSize))
                    ).SelectMany(i => i)
            ).SelectMany(i => i)
        ).SelectMany(i => i).ToHashSet();

    
    private static bool IsWithinBounds(Coordinate coord, Coordinate gridSize)
        => coord.X < gridSize.X && coord.X >= 0 && coord.Y < gridSize.Y && coord.Y >= 0;
}