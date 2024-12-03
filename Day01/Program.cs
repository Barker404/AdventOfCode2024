using System.Diagnostics;

namespace Day01;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        Tuple<List<int>, List<int>> locationLists = ReadInput(inputPath);
        int totalDistance = GetTotalDistance(locationLists.Item1, locationLists.Item2);
        Console.WriteLine($"Total distance: {totalDistance}");
        int similarityScore = GetSimilarityScore(locationLists.Item1, locationLists.Item2);
        Console.WriteLine($"Similarity score: {similarityScore}");
    }

    private static Tuple<List<int>, List<int>> ReadInput(string filePath)
    {
        List<int> locations1 = [];
        List<int> locations2 = [];

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new Exception("Unexpected number of items in line");
            }

            locations1.Add(int.Parse(parts[0]));
            locations2.Add(int.Parse(parts[1]));
        }

        return new Tuple<List<int>, List<int>>(locations1, locations2);
    }

    private static int GetTotalDistance(List<int> locations1, List<int> locations2)
    {
        Debug.Assert(locations1.Count == locations2.Count);
        return locations1.Order().Zip(locations2.Order(), (l1, l2) => Math.Abs(l1 - l2)).Sum();
    }

    private static int GetSimilarityScore(List<int> locations1, List<int> locations2)
    {
        Debug.Assert(locations1.Count == locations2.Count);
        Dictionary<int, int> locations2Counts = locations2.GroupBy(i => i).ToDictionary(
            group => group.Key, group => group.Count()
        );
        return locations1.Select(i => i * locations2Counts.GetValueOrDefault(i)).Sum();
    }
}