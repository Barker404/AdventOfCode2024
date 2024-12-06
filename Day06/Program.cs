namespace Day06;

class Program
{
    private enum Direction
    {
        Down,
        Up,
        Left,
        Right
    }

    private static readonly Dictionary<Direction, Coordinate> DirectionMoves = new()
    {
        [Direction.Down] = new Coordinate(0, 1),
        [Direction.Up] = new Coordinate(0, -1),
        [Direction.Left] = new Coordinate(-1, 0),
        [Direction.Right] = new Coordinate(1, 0),
    };
    
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        Dictionary<Coordinate, bool> inputMap = ReadInputMap(inputPath);
        Coordinate start = ReadInputStartPosition(inputPath);

        var visitedSpaces = GetSpacesVisited(inputMap, start);
        if (visitedSpaces == null)
        {
            throw new Exception("Loop detected");
        }
        int spacesVisited = visitedSpaces.Count;
        Console.WriteLine($"Visited spaces: {spacesVisited}");

        int loopingObstacleCount = visitedSpaces.Select(
            coordinate => CopyMapWithObstacle(inputMap, coordinate)
        ).Select(
            newMap => GetSpacesVisited(newMap, start)
        ).Count(output => output == null);
        
        Console.WriteLine($"Looping obstacle count: {loopingObstacleCount}");
    }

    private static Dictionary<Coordinate, bool> ReadInputMap(string inputPath)
    {
        Dictionary<Coordinate, bool> map = [];
        int y = 0;
        foreach (string line in File.ReadLines(inputPath))
        {
            int x = 0;
            foreach (char c in line)
            {
                map[new Coordinate(x, y)] = ReadInputChar(c);
                x += 1;
            }
            y += 1;
        }

        return map;
    }

    private static bool ReadInputChar(char input)
    {
        return input switch
        {
            '.' => false,
            '#' => true,
            '^' => false,
            _ => throw new Exception("Invalid character")
        };
    }

    private static Coordinate ReadInputStartPosition(string inputPath)
    {
        int y = 0;
        foreach (string line in File.ReadLines(inputPath))
        {
            int x = 0;
            foreach (char c in line)
            {
                if (c == '^')
                {
                    return new Coordinate(x, y);
                }
                x += 1;
            }
            y += 1;
        }

        throw new Exception("No start position in input");
    }

    private static Direction TurnRight(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new ArgumentException("Invalid argument")
        };
    }

    private static HashSet<Coordinate>? GetSpacesVisited(Dictionary<Coordinate, bool> obstacleMap, Coordinate start)
    {
        Direction currentDirection = Direction.Up;
        Coordinate currentPosition = start;

        Dictionary<Coordinate, HashSet<Direction>> visitedMap = new()
        {
            [start] = [currentDirection],
        };
        
        while (obstacleMap.ContainsKey(currentPosition + DirectionMoves[currentDirection]))
        {
            if (obstacleMap[currentPosition + DirectionMoves[currentDirection]])
            {
                currentDirection = TurnRight(currentDirection);
            }
            else
            {
                currentPosition += DirectionMoves[currentDirection];
                if (visitedMap.ContainsKey(currentPosition))
                {
                    if (visitedMap[currentPosition].Contains(currentDirection))
                    {
                        return null;
                    }
                    visitedMap[currentPosition].Add(currentDirection);
                }
                else
                {
                    visitedMap[currentPosition] = [currentDirection];
                }
            }
        }

        return visitedMap.Select(entry => entry.Key).ToHashSet();
    }

    private static Dictionary<Coordinate, bool> CopyMapWithObstacle(Dictionary<Coordinate, bool> inputMap,
        Coordinate obstacle)
    {
        Dictionary<Coordinate, bool> newMap = inputMap.ToDictionary();
        newMap[obstacle] = true;
        return newMap;
    }
}