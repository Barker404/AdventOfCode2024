namespace Day11;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        
        List<long> stones = ReadInput(inputPath);
        
        Dictionary<int, Dictionary<long, long>> memo = [];

        int blinkCount = 25;
        long totalStones = stones.Select(s => GetStoneCount(s, blinkCount, memo)).Sum();
        Console.WriteLine($"Total stones after {blinkCount} blinks: {totalStones}");
        
        blinkCount = 75;
        totalStones = stones.Select(s => GetStoneCount(s, blinkCount, memo)).Sum();
        Console.WriteLine($"Total stones after {blinkCount} blinks: {totalStones}");
    }

    private static List<long> ReadInput(string inputPath)
    {
        return [..File.ReadAllLines(inputPath)[0].Split(" ").Select(long.Parse)];
    }

    private static long GetStoneCount(long startStone, int steps, Dictionary<int, Dictionary<long, long>> memo)
    {
        if (steps == 0)
        {
            return 1;
        }
        
        if (!memo.ContainsKey(steps))
        {
            memo.Add(steps, new Dictionary<long, long>());
        }
        
        if (memo[steps].ContainsKey(startStone))
        {
            return memo[steps][startStone];
        }
        else
        {
            long total = GetNewStones(startStone).Select(s => GetStoneCount(s, steps - 1, memo)).Sum();
            memo[steps][startStone] = total;
            return total;
        }
    }

    private static IEnumerable<long> GetNewStones(long stone)
    {
        if (stone == 0)
        {
            return [1];
        }
        
        string stringValue = stone.ToString();
        if (stringValue.Length % 2 == 0)
        {
            long newStoneValue1 = long.Parse(stringValue[..(stringValue.Length / 2)]);
            long newStoneValue2 = long.Parse(stringValue[(stringValue.Length / 2)..]);
            return [newStoneValue1, newStoneValue2];
        }
        
        return [stone * 2024];
    }
}
