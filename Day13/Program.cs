using System.Text.RegularExpressions;

namespace Day13;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        List<((long x, long y) a, (long x, long y) b, (long x, long y) p)> problems = ReadInput(inputPath);
        
        long solutionPriceSum = problems.Select(
            problem => SolveProblem(problem.a, problem.b, problem.p)
        ).Where(
            solution => solution.HasValue
        ).Select(
            solution => solution!.Value
        ).Select(
            solution => GetPrice(solution.a, solution.b)
        ).Sum();
        
        Console.WriteLine($"Sum of all solutions: {solutionPriceSum}");
        
        List<((long x, long y) a, (long x, long y) b, (long x, long y) p)> convertedProblems = problems.Select(
            problem => (problem.a, problem.b, ApplyUnitConversion(problem.p.x, problem.p.y))
        ).ToList();
        
        long convertedSolutionPriceSum = convertedProblems.Select(
            problem => SolveProblem(problem.a, problem.b, problem.p)
        ).Where(
            solution => solution.HasValue
        ).Select(
            solution => solution!.Value
        ).Select(
            solution => GetPrice(solution.a, solution.b)
        ).Sum();
        
        Console.WriteLine($"Sum of all converted problem solutions: {convertedSolutionPriceSum}");
    }

    private static List<((long x, long y) a, (long x, long y) b, (long x, long y) p)> ReadInput(string inputPath)
    {
        string[] lines = File.ReadAllLines(inputPath).Where(line => line.Length > 0).ToArray();
        if (lines.Length % 3 != 0)
        {
            throw new Exception("Unexpected input length");
        }

        Regex buttonARegex = new Regex(@"^Button A: X\+(\d+), Y\+(\d+)$");
        Regex buttonBRegex = new Regex(@"^Button B: X\+(\d+), Y\+(\d+)$");
        Regex prizeRegex = new Regex(@"^Prize: X=(\d+), Y=(\d+)$");

        List<((long x, long y) a, (long x, long y) b, (long x, long y) p)> problems = [];
        
        for (int i = 0; i < lines.Length; i += 3)
        {
            Match buttonAMatch = buttonARegex.Match(lines[i]);
            Match buttonBMatch = buttonBRegex.Match(lines[i + 1]);
            Match prizeMatch = prizeRegex.Match(lines[i + 2]);

            if (!buttonAMatch.Success || !buttonBMatch.Success || !prizeMatch.Success)
            {
                throw new Exception($"Could not parse at line {i}");
            }
            
            problems.Add((
                (long.Parse(buttonAMatch.Groups[1].Value), long.Parse(buttonAMatch.Groups[2].Value)),
                (long.Parse(buttonBMatch.Groups[1].Value), long.Parse(buttonBMatch.Groups[2].Value)),
                (long.Parse(prizeMatch.Groups[1].Value), long.Parse(prizeMatch.Groups[2].Value))
            ));
        }

        return problems;
    }

    private static (long x, long y) ApplyUnitConversion(long x, long y)
    {
        const long addition = 10000000000000;
        return (x + addition, y + addition);
    }
    
    private static (long a, long b)? SolveProblem((long x, long y) a, (long x, long y) b, (long x, long y) p)
    {
        // Equations, solving for s.x and s.y:
        // a.x * s.a + b.x * s.b = p.x
        // a.y * s.a + b.y * s.b = p.y

        if (a.x * b.y - b.x * a.y == 0)
        {
            // Multiple possible solutions (a and b are proportional) so would need to solve with different approach
            // to find price-optimal solution
            // Thankfully not needed for given problems!
            throw new NotImplementedException();
        }

        return SolveWithCramersRule(a, b, p);
    }

    private static (long a, long b)? SolveWithCramersRule((long x, long y) a, (long x, long y) b, (long x, long y) p)
    {
        // https://en.wikipedia.org/wiki/Cramer%27s_rule#Explicit_formulas_for_small_systems
        
        long denominator = a.x * b.y - b.x * a.y;
        if (denominator == 0) throw new ArgumentException("Cannot solve this equation with cramer's rule");
        
        long numeratorA = p.x * b.y - b.x * p.y;
        long numeratorB = a.x * p.y - p.x * a.y;

        // Solution is not integer, so not useful for this scenario
        if (numeratorA % denominator != 0 || numeratorB % denominator != 0) return null;

        return (numeratorA / denominator, numeratorB / denominator);
    }

    private static long GetPrice(long a, long b)
    {
        return a * 3 + b;
    }
}