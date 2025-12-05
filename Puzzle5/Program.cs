using System.Numerics;

(List<(long, long)> ranges, List<long> items) = ParseFile("input.txt");

part2(ranges);
//part1(ranges, items);

#region part2
void part2(List<(long, long)> ranges) {
    var optimizedRanges = optimizeRanges(ranges);

    BigInteger sum = 0;
    foreach (var range in optimizedRanges) {
        sum += (range.Item2 - range.Item1 + 1);
    }

    Console.WriteLine($"count {sum}");
}

List<(long, long)> optimizeRanges(List<(long, long)> ranges) {
    var queue =  new Queue<(long, long)>(ranges);
    var result = new List<(long, long)>(ranges);

    foreach (var (start, end) in queue) {
        // remove temporarily in the current results
        result.Remove((start, end));

        // is there any overlap in the current(!) result set?
        var overlap = result
            .Where(x => inRange(start, x))
            .FirstOrDefault();
        if ((overlap.Item1 == 0) && (overlap.Item2 == 0)) {
            // ...no, then re-add it
            result.Add((start, end));
        } else {
            // ...yes, then combine the current and the overlap
            result.Remove(overlap);
            result.Add((Math.Min(start, overlap.Item1), Math.Max(end, overlap.Item2)));
        }
    }

    // repeat until no modification was done
    if (ranges.Count == result.Count) {
        return result;
    } else {
        return optimizeRanges(result);
    }
}
#endregion

#region part1
void part1(List<(long, long)> ranges, List<long> items) {
    var count = items.Where(num => inAnyRange(num, ranges)).Count();
    Console.WriteLine($"count {count}");

    bool inAnyRange(long num, List<(long, long)> ranges) {
        return ranges.Where(range => inRange(num, range)).Any();
    }
}
#endregion

bool inRange(long num, (long, long) range) {
    return (range.Item1 <= num && num <= range.Item2);
}

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