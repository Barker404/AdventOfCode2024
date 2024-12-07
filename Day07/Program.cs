namespace Day07;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        List<Tuple<long, List<int>>> inputs = ReadInputs(inputPath).ToList();

        long totalPossibleTwoOp = inputs.Where(
            t => IsCalculationPossibleTwoOp(t.Item1, t.Item2)
        ).Sum(t => t.Item1);
        Console.WriteLine($"Sum of possible calculations with 2 operations: {totalPossibleTwoOp}");
        
        
        long totalPossibleThreeOp = inputs.Where(
            t => IsCalculationPossibleThreeOp(t.Item1, 0, t.Item2)
        ).Sum(t => t.Item1);
        Console.WriteLine($"Sum of possible calculations with 3 operations: {totalPossibleThreeOp}");
    }

    private static IEnumerable<Tuple<long, List<int>>> ReadInputs(string inputPath)
    {
        foreach (string line in File.ReadAllLines(inputPath))
        {
            string[] parts = line.Split(":", 2);
            long target = long.Parse(parts[0]);
            List<int> values = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            yield return new Tuple<long, List<int>>(target, values);
        }
    }

    private static bool IsCalculationPossibleTwoOp(long target, List<int> values)
    {
        return GeneratePermutations(values.Count - 1).Select(
            operators => CheckCalculationTwoOp(target, values, operators)
        ).Any(b => b);
    }

    private static bool CheckCalculationTwoOp(long target, List<int> values, bool[] operators)
    {
        long result = values[0];
        for (int i = 0; i < operators.Length; i++)
        {
            if (operators[i])
            {
                result *= values[i + 1];
            }
            else
            {
                result += values[i + 1];
            }

            if (result > target) return false;
        }

        return result == target;
    }

    private static IEnumerable<bool[]> GeneratePermutations(int size)
    {
        int count = (int)Math.Pow(2, size);
        return Enumerable.Range(0, count)
            .Select(i =>
                Enumerable.Range(0, size)
                    .Select(pos => (i & (1 << pos)) > 0)
                    .ToArray()
            );
    }
    
    private static bool IsCalculationPossibleThreeOp(long target, long currentTotal, List<int> remainingValues)
    {
        if (currentTotal > target)
        {
            return false;
        }

        if (remainingValues.Count == 0)
        {
            return currentTotal == target;
        }
        
        List<int> nextRemainingValues = remainingValues.Slice(1, remainingValues.Count - 1);
        
        // Concat is probably the most expensive op, so try last.
        // Multiply will hit the early return sooner than add, so try first.
        if (IsCalculationPossibleThreeOp(
                target, currentTotal * remainingValues[0], nextRemainingValues
        )) return true;
        if (IsCalculationPossibleThreeOp(
                target, currentTotal + remainingValues[0], nextRemainingValues
        )) return true;
        if (IsCalculationPossibleThreeOp(
                target, ConcatOp(currentTotal, remainingValues[0]), nextRemainingValues
        )) return true;
        
        return false;
    }

    // There's a more efficient way with multiplying by powers of 10 if needed
    private static long ConcatOp(long l1, long l2)
        => long.Parse(l1.ToString() + l2.ToString());
}
