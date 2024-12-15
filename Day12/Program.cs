namespace Day12;

class Program
{
    private enum Direction
    {
        Down,
        Up,
        Left,
        Right
    }

    private static readonly (Direction d1, Direction d2)[] CornerDirections =
    [
        (Direction.Up, Direction.Right),
        (Direction.Right, Direction.Down),
        (Direction.Down, Direction.Left),
        (Direction.Left, Direction.Up),
    ];

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        (char[] gridArray, int width, int height) = ReadInput(inputPath);
        int fencingPrice = GetFencingPrice(gridArray, width, height, useDiscount: false);
        Console.WriteLine($"Total fencing price: {fencingPrice}");

        int fencingPriceWithDiscount = GetFencingPrice(gridArray, width, height, useDiscount: true);
        Console.WriteLine($"Total fencing price with discount: {fencingPriceWithDiscount}");
    }


    private static (char[] gridArray, int width, int height) ReadInput(string inputPath)
    {
        string[] lines = File.ReadAllLines(inputPath);
        if (lines.Length < 1)
        {
            throw new Exception("Empty grid");
        }

        if (lines.Any(line => line.Length != lines[0].Length))
        {
            throw new Exception("Grid not rectangular");
        }

        return (lines.SelectMany(i => i).ToArray(), lines[0].Length, lines.Length);
    }

    private static int GetFencingPrice(char[] gridArray, int width, int height, bool useDiscount)
    {
        bool[] isCheckedArea = new bool[gridArray.Length];
        bool[] isCheckedPerimeter = new bool[gridArray.Length];
        
        return Enumerable.Range(0, gridArray.Length).Where(
            i => !isCheckedArea[i]
        ).Select(
            i => GetRegionArea(gridArray, i, width, height, isCheckedArea) *
                 (useDiscount
                     ? GetRegionCorners(gridArray, i, width, height, isCheckedPerimeter)
                     : GetRegionPerimeter(gridArray, i, width, height, isCheckedPerimeter)
                 )
        ).Sum();
    }

    private static int GetRegionArea(char[] gridArray, int i, int width, int height, bool[] isChecked)
    {
        isChecked[i] = true;

        return Enum.GetValues<Direction>().Where(
            direction => CanMove(i, direction, width, height)
        ).Select(
            direction => Move(i, direction, width)
        ).Where(
            adjacent => gridArray[adjacent] == gridArray[i] && !isChecked[adjacent]
        ).Select(
            adjacent => GetRegionArea(gridArray, adjacent, width, height, isChecked)
        ).Sum() + 1;
    }

    private static int GetRegionPerimeter(char[] gridArray, int i, int width, int height, bool[] isChecked)
    {
        isChecked[i] = true;

        List<int> adjacentSameArea = Enum.GetValues<Direction>().Where(
            direction => CanMove(i, direction, width, height)
        ).Select(
            direction => Move(i, direction, width)
        ).Where(
            adjacent => gridArray[adjacent] == gridArray[i]
        ).ToList();

        return adjacentSameArea.Where(
            adjacent => !isChecked[adjacent]
        ).Select(
            adjacent => GetRegionPerimeter(gridArray, adjacent, width, height, isChecked)
        ).Sum() + 4 - adjacentSameArea.Count;
    }

    private static int GetRegionCorners(char[] gridArray, int i, int width, int height, bool[] isChecked)
    {
        isChecked[i] = true;
        
        int thisCorners = CornerDirections.Count(
            dPair => IsCorner(gridArray, i, dPair.d1, dPair.d2, width, height)
        );

        return Enum.GetValues<Direction>().Where(
            direction => CanMove(i, direction, width, height)
        ).Select(
            direction => Move(i, direction, width)
        ).Where(
            adjacent => gridArray[adjacent] == gridArray[i] && !isChecked[adjacent]
        ).Select(
            adjacent => GetRegionCorners(gridArray, adjacent, width, height, isChecked)
        ).Sum() + thisCorners;
    }

    private static bool IsCorner(char[] gridArray, int i, Direction d1, Direction d2, int width, int height)
    {
        char? adjacentValue1 = CanMove(i, d1, width, height) ? gridArray[Move(i, d1, width)] : null;
        char? adjacentValue2 = CanMove(i, d2, width, height) ? gridArray[Move(i, d2, width)] : null;
        char? diagonalValue = adjacentValue1 != null && adjacentValue2 != null
            ? gridArray[Move(Move(i, d1, width), d2, width)]
            : null;
        char thisValue = gridArray[i];

        return (adjacentValue1 != thisValue && adjacentValue2 != thisValue) || // Outer
               (adjacentValue1 == thisValue && adjacentValue2 == thisValue && diagonalValue != thisValue); // Inner
    }

    private static (int x, int y) GetCoords(int i, int width)
    {
        return (i % width, i / width);
    }

    private static int GetIndex(int x, int y, int width)
    {
        return x + width * y;
    }

    private static bool CanMove(int i, Direction direction, int width, int height)
    {
        return i >= 0 &&
               i < width * height &&
               !(direction == Direction.Up && i < width) &&
               !(direction == Direction.Down && i >= width * (height - 1)) &&
               !(direction == Direction.Left && i % width == 0) &&
               !(direction == Direction.Right && i % width == width - 1);
    }

    private static int Move(int i, Direction direction, int width)
    {
        return direction switch
        {
            Direction.Down => i + width,
            Direction.Up => i - width,
            Direction.Right => i + 1,
            Direction.Left => i - 1,
            _ => throw new Exception("Invalid direction")
        };
    }
}