(List<(long, long)> ranges, List<long> items) = ParseFile("input.txt");

part1(ranges, items);

#region part1
void part1(List<(long, long)> ranges, List<long> items) {
    var count = items.Where(num => inAnyRange(num, ranges)).Count();
    Console.WriteLine($"count {count}");

    bool inAnyRange(long num, List<(long, long)> ranges) {
        return ranges.Where(range => inRange(num, range)).Any();
    }
    bool inRange(long num, (long, long) range) {
        return (range.Item1 <= num && num <= range.Item2);
    }
}
#endregion

(List<(long, long)>, List<long>) ParseFile(string filePath) {
    // Read all lines from the file
    string[] lines = File.ReadAllLines(filePath);

    var ranges = new List<(long, long)>();
    var items = new List<long>();

    foreach (var line in lines) {
        if (String.IsNullOrEmpty(line)) {
            continue;
        }

        var parts = line.Split('-');
        if (parts.Length == 1) {
            items.Add(long.Parse(line));
        } else if (parts.Length == 2) {
            ranges.Add((long.Parse(parts[0]), long.Parse(parts[1])));
        }
    }

    return (ranges, items);
}