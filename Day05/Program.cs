using System.Text.RegularExpressions;

namespace Day05;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        string[] inputLines = ReadInputLines(inputPath);
        Dictionary<int, List<int>> rules = ReadRules(inputLines);
        List<List<int>> manualUpdates = ReadManualUpdates(inputLines);

        List<List<int>> correctManualUpdates = manualUpdates.Where(
            m => IsManualUpdateCorrect(m, rules)
        ).ToList();
        List<List<int>> incorrectManualUpdates = manualUpdates.Where(
            m => !IsManualUpdateCorrect(m, rules)
        ).ToList();
        int correctManualUpdateSum = correctManualUpdates.Sum(GetMiddlePage);
        Console.WriteLine($"Sum of correct manual update middle pages: {correctManualUpdateSum}");

        List<List<int>> fixedManualUpdates = incorrectManualUpdates.Select(
            m => FixManualUpdate(m, rules)
        ).ToList();
        int fixedManualUpdateSum = fixedManualUpdates.Sum(GetMiddlePage);
        Console.WriteLine($"Sum of fixed incorrect manual update middle pages: {fixedManualUpdateSum}");

    }

    private static string[] ReadInputLines(string inputPath)
    {
        return File.ReadAllLines(inputPath);
    }

    /// <summary>
    /// Rules reversed - entry x -> y means y cannot come after x in sequence
    /// </summary>
    private static Dictionary<int, List<int>> ReadRules(IEnumerable<string> lines)
    {
        Dictionary<int, List<int>> rules = new Dictionary<int, List<int>>();
        Regex regex = new Regex(@"^(\d+)\|(\d+)$");
        
        foreach (string line in lines)
        {
            Match regexMatch;
            if ((regexMatch = regex.Match(line)).Success)
            {
                int first = int.Parse(regexMatch.Groups[1].Value);
                int second = int.Parse(regexMatch.Groups[2].Value);
                    
                if (rules.ContainsKey(second))
                {
                    rules[second].Add(first);
                }
                else
                {
                    rules[second] = [first];
                }
            }
        }

        return rules;
    }

    private static List<List<int>> ReadManualUpdates(IEnumerable<string> lines)
    {
        return lines.Where(line => line.Contains(',')).Select(
            line => line.Split(",").Select(int.Parse).ToList()
        ).ToList();
    }


    private static int GetMiddlePage(List<int> manualUpdate)
    {
        if (manualUpdate.Count % 2 == 0)
        {
            throw new Exception("Even number of pages in valid manual");
        }
        return manualUpdate[manualUpdate.Count / 2];
    }
    
    private static bool IsManualUpdateCorrect(List<int> manualUpdate, Dictionary<int, List<int>> rules)
    {
        // Build a set of pages that would make the update incorrect if they occurred
        // - those which cannot appear after already seen pages
        HashSet<int> disallowedPages = [];
        
        foreach (int pageNumber in manualUpdate)
        {
            if (disallowedPages.Contains(pageNumber)) return false;
            if (rules.ContainsKey(pageNumber))
            {
                rules[pageNumber].ForEach(i => disallowedPages.Add(i));
            }
        }

        return true;
    }
    
    private static List<int> FixManualUpdate(List<int> manualUpdate, Dictionary<int, List<int>> rules)
    {
        HashSet<int> pagesToAdd = [..manualUpdate];
        List<int> fixedManualUpdate = [];
        
        while (pagesToAdd.Count != 0)
        {
            fixedManualUpdate.AddRange(FixManualUpdateDfs(pagesToAdd.First(), pagesToAdd, rules));
        }

        return fixedManualUpdate;
    }

    private static List<int> FixManualUpdateDfs(int page, HashSet<int> pagesToAdd, Dictionary<int, List<int>> rules)
    {
        // Doesn't handle loops
        
        if (!rules.ContainsKey(page))
        {
            pagesToAdd.Remove(page);
            return [page];
        }

        List<int> returnValue = [];
        foreach (int otherPage in rules[page])
        {
            // otherPage must come before page
            if (pagesToAdd.Contains(otherPage))
            {
                // Also get any pagesToAdd that must come before otherPage
                returnValue.AddRange(FixManualUpdateDfs(otherPage, pagesToAdd, rules));
            }
        }
        
        pagesToAdd.Remove(page);
        returnValue.Add(page);
        return returnValue;
    }
}