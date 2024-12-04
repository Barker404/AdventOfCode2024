using System.Text.RegularExpressions;

namespace Day03;

class Program
{
    private record MultiplyInstruction(int Num1, int Num2)
    {
        public int Value => Num1 * Num2;
    }
    
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];
        string inputText = ReadInput(inputPath);
        List<MultiplyInstruction> multiplies = GetMultiplyInstruction(inputText, useConditional: false);
        int multipliesSum = multiplies.Sum(multiply => multiply.Value);
        Console.WriteLine($"Sum of multiply instructions: {multipliesSum}");
        List<MultiplyInstruction> multipliesWithConditional = GetMultiplyInstruction(inputText, useConditional: true);
        int multipliesWithConditionalSum = multipliesWithConditional.Sum(multiply => multiply.Value);
        Console.WriteLine($"Sum of multiply instructions with conditional: {multipliesWithConditionalSum}");
    }
    
    private static string ReadInput(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    private static List<MultiplyInstruction> GetMultiplyInstruction(string input, bool useConditional)
    {
        // \G requires match to be at start of text being searched
        Regex mulRegex = new Regex(@"\Gmul\((\d{1,3}),(\d{1,3})\)");
        const string doCommand = "do()";
        const string dontCommand = "don't()";

        List<MultiplyInstruction> multiplies = [];
        bool enabled = true;
        int i = 0;
        
        while (i < input.Length)
        {
            if (useConditional && !enabled && MatchesAtIndex(input, doCommand, i))
            {
                enabled = true;
                i += doCommand.Length;
                continue;
            }
            if (useConditional && enabled && MatchesAtIndex(input, dontCommand, i))
            {
                enabled = false;
                i += dontCommand.Length;
                continue;
            }
            if (enabled)
            {
                Match match = mulRegex.Match(input, i);
                if (match.Success)
                {
                    multiplies.Add(
                        new MultiplyInstruction(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value))
                    );
                    i += match.Value.Length;
                    continue;
                }
            }
            i++;
        }
        
        return multiplies;
    }

    private static bool MatchesAtIndex(string stringToSearch, string targetString, int index)
    {
        if (stringToSearch.Length - index < targetString.Length)
        {
            return false;
        }

        return stringToSearch.IndexOf(targetString, index, targetString.Length, StringComparison.Ordinal) == index;
    }
}