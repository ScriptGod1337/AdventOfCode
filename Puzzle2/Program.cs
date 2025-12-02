var input = File.ReadAllText("input.txt");

var ranges = input
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(s => s.Trim().Split('-'))
    .Select(p => (Start: long.Parse(p[0]), End: long.Parse(p[1])))
    .ToList();

Console.WriteLine($"overallSum1 {part(isFake1)}");
Console.WriteLine($"overallSum2 {part(isFake2)}");

long part(Func<long, bool> fakeFunction) {
    long overallSum = 0;
    foreach ((var start, var end) in ranges) {
        var sum = LongRange(start, end).ToList().Where(fakeFunction).Sum();
        // Console.WriteLine($"Sum {sum}");
        overallSum += sum;
    }

    return overallSum; 
}

#region part2
bool isFake2(long number) {
    var numStr = number.ToString();
    for (int n = 1; n < numStr.Length; n++) {
        var pattern = numStr.Substring(0, n);
        var replaced = numStr.Replace(pattern, String.Empty);
        if (replaced.Length == 0) {
            return true;
        }
    }

    return false;
}
#endregion

#region part1
bool isFake1(long number) {
    var digits = Math.Floor(Math.Log10(number)) + 1;
    if (digits % 2 != 0) {
        return false;
    }

    var half = digits / 2;
    var pow = (long) Math.Pow(10, half);

    var left  = number / pow;
    var right = number % pow;
    return (left == right);
}
#endregion

IEnumerable<long> LongRange(long start, long end) {
    if (start <= end) {
        for (long i = start; i <= end; i++)
            yield return i;
    } else {
        for (long i = start; i >= end; i--)
            yield return i;
    }
}