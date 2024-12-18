using System.Diagnostics;

namespace Day15;

class Program
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private static readonly Dictionary<char, Direction> CharToDirection = new ()
    {
        ['^'] = Direction.Up,
        ['v'] = Direction.Down,
        ['<'] = Direction.Left,
        ['>'] = Direction.Right,
    };
    private static readonly Dictionary<Direction, Coordinate> DirectionToMove = new ()
    {
        [Direction.Up] = new Coordinate(0, -1),
        [Direction.Down] = new Coordinate(0, 1),
        [Direction.Left] = new Coordinate(-1, 0),
        [Direction.Right] = new Coordinate(1, 0),
    };
    

    private const char StartPositionChar = '@';
    private const char Wall = '#';
    private const char Box = 'O';
    private const char BigBoxLeft = '[';
    private const char BigBoxRight = ']';
    private const char Empty = '.';

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        Dictionary<Coordinate, char> grid = ReadInputGrid(inputPath);
        Coordinate startPosition = ReadInputStartPosition(inputPath);
        Dictionary<Coordinate, char> wideGrid = GetWideGrid(grid);
        Coordinate wideStartPosition = GetWideStartPosition(startPosition);
        List<Direction> moves = ReadInputMoves(inputPath);

        ApplyMoves(grid, startPosition, moves);
        
        int boxGpsSum = grid.Where(
            entry => entry.Value == Box
        ).Select(
            entry => GetGpsCoordinate(entry.Key)
        ).Sum();
        
        Console.WriteLine($"Sum of box GPS values: {boxGpsSum}");
        
        ApplyMoves(wideGrid, wideStartPosition, moves);
        
        int bigBoxGpsSum = wideGrid.Where(
            entry => entry.Value == BigBoxLeft
        ).Select(
            entry => GetGpsCoordinate(entry.Key)
        ).Sum();
        
        Console.WriteLine($"Sum of big box GPS values in wide grid: {bigBoxGpsSum}");
    }

    private static Dictionary<Coordinate, char> ReadInputGrid(string inputPath)
    {
        List<string> lines = File.ReadLines(inputPath).TakeWhile(line => line.Contains(Wall)).ToList();

        if (lines.Count == 0 || lines.Any(line => line.Length != lines[0].Length))
        {
            throw new Exception("Invalid input grid");
        }

        return lines.Select(
            (line, y) => line.Select(
                (c, x) => (new Coordinate(x, y), c)
            )
        ).SelectMany(
            x => x
        ).Where(
            // Ignore start position and empty spaces (sparse grid)
            pair => pair.Item2 != StartPositionChar && pair.Item2 != Empty
        ).ToDictionary(
            pair => pair.Item1,
            pair => pair.Item2
        );
    }

    private static Dictionary<Coordinate, char> GetWideGrid(Dictionary<Coordinate, char> originalGrid)
    {
        return originalGrid.Select(
            entry =>
                (entry.Key with { X = entry.Key.X * 2 }, entry.Value == Box ? BigBoxLeft : entry.Value)
        ).Concat(
            originalGrid.Select(
                entry =>
                    (entry.Key with { X = entry.Key.X * 2 + 1 }, entry.Value == Box ? BigBoxRight : entry.Value)
                )
        ).ToDictionary(
            pair => pair.Item1,
            pair => pair.Item2
        );
    }

    private static Coordinate ReadInputStartPosition(string inputPath)
    {
        List<string> lines = File.ReadLines(inputPath).TakeWhile(line => line.Contains(Wall)).ToList();

        for (int y = 0; y < lines.Count; y++)
        {
            if (lines[y].Contains(StartPositionChar))
            {
                return new Coordinate(lines[y].IndexOf(StartPositionChar), y);
            }
        }

        throw new Exception("No start position found in input grid");
    }
    
    private static Coordinate GetWideStartPosition(Coordinate originalStartPosition)
    {
        return originalStartPosition with { X = originalStartPosition.X * 2 };
    }

    private static List<Direction> ReadInputMoves(string inputPath)
    {
        List<string> lines = File.ReadLines(inputPath).Where(line => line.Length != 0 && !line.Contains(Wall)).ToList();

        if (lines.Count == 0)
        {
            throw new Exception("No moves found in input");
        }

        return lines.SelectMany(c => c).Select(c => CharToDirection[c]).ToList();
    }

    private static void ApplyMoves(Dictionary<Coordinate, char> grid, Coordinate startPosition, List<Direction> moves)
    {
        Coordinate position = startPosition;
        // PrintGrid(grid, position);
        
        foreach (Direction move in moves)
        {
            position = TryMove(grid, position, move);
            // PrintGrid(grid, position);
        }
    }
    
    private static Coordinate TryMove(Dictionary<Coordinate, char> grid, Coordinate position, Direction direction)
    {
        Coordinate target = position + DirectionToMove[direction];
        bool canMove = CanMoveInto(grid, target, direction);
        
        if (grid.ContainsKey(target) && canMove)
        {
            MoveObject(grid, target, direction);
        }

        Debug.Assert(!canMove || !grid.ContainsKey(target));
        return canMove ? target : position;
    }

    private static bool CanMoveInto(Dictionary<Coordinate, char> grid, Coordinate position, Direction direction)
    {
        if (!grid.ContainsKey(position))
        {
            return true;
        }

        Coordinate target = position + DirectionToMove[direction];

        if (grid[position] == Wall)
        {
            return false;
        }

        if (grid[position] == BigBoxLeft && direction is Direction.Up or Direction.Down)
        {
            return CanMoveInto(grid, target, direction) &&
                   CanMoveInto(grid, target + DirectionToMove[Direction.Right], direction);
        }
        
        if (grid[position] == BigBoxRight && direction is Direction.Up or Direction.Down)
        {
            return CanMoveInto(grid, target, direction) &&
                   CanMoveInto(grid, target + DirectionToMove[Direction.Left], direction);
        }

        // Box or horizontal big box
        return CanMoveInto(grid, target, direction);
    }
    
    private static void MoveObject(Dictionary<Coordinate, char> grid, Coordinate position, Direction direction)
    {
        if (!grid.ContainsKey(position))
        {
            throw new Exception("Nothing to move");
        }

        switch (grid[position])
        {
            case Box:
                MoveBox(grid, position, direction);
                break;
            case BigBoxLeft:
            case BigBoxRight:
                MoveBigBox(grid, position, direction);
                break;
            case Wall:
                throw new Exception("Tried to move a wall");
            default:
                throw new Exception("Unexpected grid value");
        }
    }

    private static void MoveBox(Dictionary<Coordinate, char> grid, Coordinate position, Direction direction)
    {
        if (!grid.ContainsKey(position) || grid[position] != Box)
        {
            throw new Exception("Tried to move a box that doesn't exist");
        }
        
        Coordinate target = position + DirectionToMove[direction];
        if (grid.ContainsKey(target))
        {
            MoveObject(grid, target, direction);
        }

        Debug.Assert(!grid.ContainsKey(target));
        grid.Remove(position);
        grid[target] = Box;
    }
    
    private static void MoveBigBox(Dictionary<Coordinate, char> grid, Coordinate position, Direction direction)
    {
        if (!grid.ContainsKey(position) || (grid[position] != BigBoxLeft && grid[position] != BigBoxRight))
        {
            throw new Exception("Tried to move a big box that doesn't exist");
        }

        Coordinate otherPartPosition = grid[position] == BigBoxLeft
            ? position + DirectionToMove[Direction.Right]
            : position + DirectionToMove[Direction.Left];
        
        Coordinate targetThis = position + DirectionToMove[direction];
        Coordinate targetOther = otherPartPosition + DirectionToMove[direction];
        
        if (grid.ContainsKey(targetOther))
        {
            MoveObject(grid, targetOther, direction);
        }
        Debug.Assert(!grid.ContainsKey(targetOther));
        grid[targetOther] = grid[otherPartPosition];
        grid.Remove(otherPartPosition);
        
        if (grid.ContainsKey(targetThis))
        {
            MoveObject(grid, targetThis, direction);
        }
        Debug.Assert(!grid.ContainsKey(targetThis));
        grid[targetThis] = grid[position];
        grid.Remove(position);
    }

    private static void PrintGrid(Dictionary<Coordinate, char> grid, Coordinate position)
    {
        if (grid.ContainsKey(position))
        {
            throw new Exception("Current position is on object");
        }

        int maxX = grid.Max(entry => entry.Key.X);
        int maxY = grid.Max(entry => entry.Key.Y);

        foreach (int y in Enumerable.Range(0, maxY + 1))
        {
            Console.WriteLine(string.Join(
                "",
                Enumerable.Range(0, maxX + 1).Select(
                    x => new Coordinate(x, y)
                ).Select(
                    coord => coord == position ? '@' : (grid.ContainsKey(coord) ? grid[coord] : '.')
                )
            ));
        }
    }
    
    private static int GetGpsCoordinate(Coordinate coordinate)
    {
        return coordinate.X + coordinate.Y * 100;
    }
}