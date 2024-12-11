// See https://aka.ms/new-console-template for more information
using System.Numerics;
using System.Reflection;

Console.WriteLine("Hello, World!");

var input = new List<ulong>();
foreach (var line in File.ReadLines("input.txt")) {
    input.AddRange(line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) //
        .Select(x => ulong.Parse(x)));

}

part2();

#region part2
void part2() {
    BigInteger nodesCreated = 0;
    var num2NodesCreated = new Dictionary<(BigInteger, int), BigInteger>();
    foreach (var num in input) {
        Console.WriteLine($" start  {num}");
        nodesCreated += processNode(num, 75, num2NodesCreated);
        Console.WriteLine($" end    {num}");
    }

    Console.WriteLine(nodesCreated);
    Console.WriteLine(input.Count);
    Console.WriteLine(nodesCreated + input.Count);
    Console.WriteLine("end");
}

BigInteger processNode(BigInteger value, int depth, Dictionary<(BigInteger, int), BigInteger> num2NodesCreated) {
    if ((depth == 0)) {
        num2NodesCreated[(value, depth)] = 0;
        return 0;
    }

    BigInteger nodesCreated = 0;
    if (num2NodesCreated.TryGetValue((value, depth), out nodesCreated)) {
        return nodesCreated;
    }

    var valueStr = value.ToString();
    var digitCount = valueStr.Length;
    if (value == 0) {
        var nextValue = (BigInteger)1;
        nodesCreated += processNode(nextValue, depth - 1, num2NodesCreated);
    } else if (long.IsEvenInteger(digitCount)) {
        var mid = digitCount / 2;
        var nextLeft = BigInteger.Parse(valueStr.Substring(0, mid));
        var nextRight = BigInteger.Parse(valueStr.Substring(mid));

        nodesCreated++;
        nodesCreated += processNode(nextLeft, depth - 1, num2NodesCreated);
        nodesCreated += processNode(nextRight, depth - 1, num2NodesCreated);
    } else if (value != 0) {
        var nextValue = value * 2024;
        nodesCreated += processNode(nextValue, depth - 1, num2NodesCreated);
    }

    num2NodesCreated[(value, depth)] = nodesCreated;
    return nodesCreated;
}
#endregion

#region part1
void slow() {
    var processing = new LinkedList<ulong>(input);
    for (int n = 0; n < 25; n++) {
        Console.Write($"Round {n}");

        long count = 0;
        var current = processing.First;
        while (current != null) {
            var value = current.Value;
            long digitCount = (long)Math.Floor(Math.Log10(value) + 1);
            if (value == 0) {
                current.Value = 1;
            } else if (long.IsEvenInteger(digitCount)) {
                var divisor = (ulong)Math.Pow(10, digitCount / 2);
                var nextLeft = (ulong)(value / divisor);
                var nextRight = (ulong)(value % divisor);

                processing.AddBefore(current, nextLeft);
                current.Value = nextRight;

                count++;
            } else {
                current.Value = value * 2024;
            }
            current = current.Next; // Move to the next node
            count++;
        }
        Console.WriteLine($" count {count}");
    }

    // Console.WriteLine(string.Join(" ", processing));
    Console.WriteLine(processing.Count);
}
#endregion