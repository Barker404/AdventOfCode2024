using System.Runtime.InteropServices.Marshalling;

namespace Day08;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("Must provide input path");
        }

        string inputPath = args[0];

        int[] input = ReadInput(inputPath);
        
        int[] expandedInput = GetExpandedInput(input);
        Fragment(expandedInput);
        long fragmentedChecksum = GetChecksum(expandedInput);
        Console.WriteLine($"Fragmented checksum: {fragmentedChecksum}");

        LinkedList<(int id, int length)> fileListInput = GetFileList(input);
        Rearrange(fileListInput);
        int[] rearrangedExpandedInput = GetExpandedInput(fileListInput);
        long rearrangedChecksum = GetChecksum(rearrangedExpandedInput);
        Console.WriteLine($"Checksum: {rearrangedChecksum}");
    }

    private static int[] ReadInput(string inputPath)
    {
        return File.ReadAllLines(inputPath)[0].Select(c => int.Parse(c.ToString())).ToArray();
    }

    private static int[] GetExpandedInput(int[] input)
    {
        return input.Select(
            (value, index) => Enumerable.Repeat(index % 2 == 0 ? index / 2 : -1, value)
        ).SelectMany(i => i).ToArray();
    }
    
    private static int[] GetExpandedInput(LinkedList<(int id, int length)> fileListInput)
    {
        return fileListInput.Select(
            file => Enumerable.Repeat(file.id, file.length)
        ).SelectMany(i => i).ToArray();
    }

    private static LinkedList<(int id, int length)> GetFileList(int[] input)
    {
        return new LinkedList<(int id, int length)>(
            input.Select(
                (value, index) => (index % 2 == 0 ? index / 2 : -1, value)
            )
        );
    }

    private static void Fragment(int[] expandedInput)
    {
        if (expandedInput.Length <= 1) return;

        int firstEmptyIndex = 0;
        int lastFilledIndex = expandedInput.Length - 1;

        while (firstEmptyIndex != lastFilledIndex)
        {
            if (expandedInput[firstEmptyIndex] >= 0)
            {
                firstEmptyIndex++;
            }
            else if (expandedInput[lastFilledIndex] < 0)
            {
                lastFilledIndex--;
            }
            else
            {
                (expandedInput[firstEmptyIndex], expandedInput[lastFilledIndex]) = (expandedInput[lastFilledIndex], expandedInput[firstEmptyIndex]);
                firstEmptyIndex++;
                lastFilledIndex--;
            }
        }
    }

    private static void Rearrange(LinkedList<(int id, int length)> fileListInput)
    {
        if (fileListInput.Count <= 1 || fileListInput.First == null || fileListInput.Last == null) return;

        LinkedListNode<(int id, int length)> fileToMove = fileListInput.Last;
        // Initialise to last non-gap
        while (fileToMove.Value.id == -1 && fileToMove.Previous != null)
        {
            fileToMove = fileToMove.Previous;
        }
        // Loop will handle initialising to the first actual gap
        LinkedListNode<(int id, int length)> firstGap = fileListInput.First;

        // Latter conditions should never fail if first condition is not yet reached
        while (firstGap != fileToMove && firstGap.Next != null && fileToMove.Previous != null)
        {
            // Step only one at a time to avoid missing loop condition
            // Could remove empty gaps, but no real need to
            if (firstGap.Value.id != -1 || firstGap.Value.length == 0)
            {
                firstGap = firstGap.Next;
            }
            else if (fileToMove.Value.id == -1)
            {
                fileToMove = fileToMove.Previous;
            }
            else
            {
                // We always go to next file back regardless of whether this one moves successfully
                // Need to get reference to next file back now in case this file switches position
                LinkedListNode<(int id, int length)> nextFileToMove = fileToMove.Previous;
                TryMoveFile(fileListInput, firstGap, fileToMove);
                fileToMove = nextFileToMove;   
            }
        }
    }

    private static void TryMoveFile(LinkedList<(int id, int length)> linkedList, LinkedListNode<(int id, int length)> firstGap, LinkedListNode<(int id, int length)> fileToMove)
    {
        LinkedListNode<(int id, int length)> candidateGap = firstGap;
        while (candidateGap != fileToMove && candidateGap.Next != null)
        {
            if (candidateGap.Value.id == -1 && candidateGap.Value.length >= fileToMove.Value.length)
            {
                // Do the move
                int moveLength = fileToMove.Value.length;

                // We might end up with consecutive empty blocks, but it's fine because they will always be on the right
                // side of the list where we're no longer trying to move things
                linkedList.AddAfter(
                    fileToMove,
                    new LinkedListNode<(int id, int length)>((-1, moveLength))
                );
                linkedList.Remove(fileToMove);
                linkedList.AddBefore(candidateGap, fileToMove);
                candidateGap.Value = (candidateGap.Value.id, candidateGap.Value.length - moveLength);

                return;
            }
            candidateGap = candidateGap.Next;
        }
    }

    private static long GetChecksum(int[] fragmentedInput)
    {
        return fragmentedInput.Select(
            (value, index) => (long)value * index
        ).Where(
            i => i >= 0
        ).Sum();
    }
}