using System.Diagnostics;

namespace Day02;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        List<List<int>> reports = ReadInput(inputPath);
        int safeReportsCount = CountSafeReports(reports, useDampener: false);
        Console.WriteLine($"Number of safe reports: {safeReportsCount}");
        int safeReportsWithDampenerCount = CountSafeReports(reports, useDampener: true);
        Console.WriteLine($"Number of safe reports with dampener: {safeReportsWithDampenerCount}");
    }

    private static List<List<int>> ReadInput(string filePath)
    {
        return File.ReadAllLines(filePath).Select(
            line => line.Split(" ").Select(int.Parse).ToList()
        ).ToList();
    }

    private static int CountSafeReports(List<List<int>> reports, bool useDampener)
    {
        return reports.Count(report => IsReportSafe(report, useDampener: useDampener));
    }

    private static bool IsReportSafe(List<int> report, bool useDampener)
    {
        int firstUnsafeLevel = GetFirstUnsafeLevel(report);
        if (firstUnsafeLevel == -1) return true;
        Debug.Assert(firstUnsafeLevel != 0); // First level should always be fine

        if (useDampener)
        {
            // Dampener can be used to:
            // * skip the first level, to reverse direction (e.g. 8 1 2 3 4)
            List<int> withoutFirst = report.Skip(1).ToList();
            if (GetFirstUnsafeLevel(withoutFirst) == -1) return true; 
            
            // * skip the first unsafe level (e.g. 1 2 8 3 4)
            List<int> withoutFirstUnsafe = report.Where((_, index) => index != firstUnsafeLevel).ToList();
            if (GetFirstUnsafeLevel(withoutFirstUnsafe) == -1) return true;
            
            // * skip the level before the first unsafe level (e.g. 1 2 4 3 4)
            // Same as first case if firstUnsafeLevel == 1
            if (firstUnsafeLevel > 1)
            {
                List<int> withoutBeforeFirstUnsafe = report.Where((_, index) => index != firstUnsafeLevel - 1).ToList();
                if (GetFirstUnsafeLevel(withoutBeforeFirstUnsafe) == -1) return true;
            }
        }

        return false;
    }

    /// <returns>-1 if report is safe, otherwise index of the first unsafe level</returns>
    private static int GetFirstUnsafeLevel(List<int> report)
    {
        if (report.Count < 2 && report[0] != report[1])
        {
            return -1;
        }

        // Don't need to worry about equal case because it will immediately be found unsafe either way
        bool isAscending = report[0] < report[1];

        for (int i = 0; i < report.Count - 1; i++)
        {
            int level1 = report[i];
            int level2 = report[i + 1];

            if ((isAscending && level1 >= level2) || (!isAscending && level1 <= level2) ||
                Math.Abs(level1 - level2) > 3)
            {
                return i + 1;
            }
        }

        return -1;
    }
    
}