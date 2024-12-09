using System.Text;
using System.Numerics;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var input = string.Join("", File.ReadAllLines("input.txt")).Select(x => int.Parse(x.ToString())).ToList();
var memory = new List<int?>();

readInput1(input, memory);
dumpMemory(memory);
defrag2(memory);
calcChecksum(memory);

void calcChecksum(List<int?> memory) {
    BigInteger sum = 0;
    for (int n = 0; n < memory.Count; n++) {
        sum += n * (memory[n] ?? 0);
    }
    Console.WriteLine(sum);
}

#region part2
void defrag2(List<int?> memory) {
    var currentPos = memory.Count - 1;
    while (currentPos > 1) {
        if (!memory[currentPos].HasValue) {
            currentPos--;
            continue;
        }

        var lastNum = memory[currentPos];
        var lastNumLen = 1;
        currentPos--;
        while (memory[currentPos].HasValue && memory[currentPos] == lastNum) {
            lastNumLen++;
            currentPos--;
        }

        var spaceLocation = FindFirstIndexOfContinuousRange(memory, currentPos, lastNumLen);
        if (spaceLocation == -1) {
            continue;
        }
        memory.SetRange(spaceLocation, lastNumLen, lastNum);
        memory.SetRange(currentPos + 1, lastNumLen, null);
        // dumpMemory(memory);
    };
    dumpMemory(memory);
}

int FindFirstIndexOfContinuousRange(List<int?> numbers, int endIndex, int minRangeLength) {
    for (int i = 0; i <= endIndex; i++) {
        // Check if the current element is the target element
        if (!numbers[i].HasValue) {
            int count = 1;

            // Check the subsequent elements for continuity
            for (int j = i + 1; j <= endIndex && numbers[j] == numbers[j - 1] + 1; j++) {
                count++;
            }

            // If the range meets the required length, return the starting index
            if (count >= minRangeLength) {
                return i;
            }
        }
    }

    return -1; // No valid range found
}
#endregion

void defrag1(List<int?> memory) {
    while (true) {
        var lastNumIdx = memory.FindLastIndex(x => x.HasValue);
        var firstEmptyIdx = memory.FindIndex(x => !x.HasValue);
        if (firstEmptyIdx >= lastNumIdx) {
            break;
        }

        (memory[lastNumIdx], memory[firstEmptyIdx]) = (memory[firstEmptyIdx], memory[lastNumIdx]);
        // dumpMemory(memory);
    };
    dumpMemory(memory);
}

void readInput1(List<int> input, List<int?> memory) {
    for (var n = 0; n < input.Count; n += 2) {
        int id = n / 2;

        var length = input[n];
        for (int i = 0; i < length; i++) {
            // Console.Write(id);
            memory.Add(id);
        }

        if (n < (input.Count - 1)) {
            var free = input[n + 1];
            for (int i = 0; i < free; i++) {
                memory.Add(null);
                // Console.Write(".");
            }
        }

    }
    Console.WriteLine();
}

void dumpMemory(List<int?> memory) {
    Console.WriteLine(string.Join("", memory.Select(x => x.HasValue ? x.Value.ToString() : ".")));
}

public static class ListExtensions {
    public static void SetRange<T>(this List<T> list, int startIndex, int length, T value) {
        for (int i = startIndex; i < startIndex + length; i++) {
            list[i] = value;
        }
    }
}