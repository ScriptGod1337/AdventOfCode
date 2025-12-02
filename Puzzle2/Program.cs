var input = File.ReadAllText("input.txt");

var ranges = input
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(s => s.Trim().Split('-'))
    .Select(p => (Start: long.Parse(p[0]), End: long.Parse(p[1])))
    .ToList();

part1();

#region part1
void part1() {
    long overallSum = 0;
    foreach ((var start, var end) in ranges) {
        var sum = LongRange(start, end).ToList().Where(isFake1).Sum();
        Console.WriteLine($"Sum {sum}");
        overallSum += sum;
    }
    Console.WriteLine($"overallSum {overallSum}");
}

bool isFake1(long number) {
    var digits = Math.Floor(Math.Log10(number)) + 1;
    if (digits % 2 != 0)
        return false;

    var half = digits / 2;
    var pow = (long) Math.Pow(10, half);

    var left  = number / pow;
    var right = number % pow;
    return left == right;  
}

IEnumerable<long> LongRange(long start, long end) {
    if (start <= end) {
        for (long i = start; i <= end; i++)
            yield return i;
    } else {
        for (long i = start; i >= end; i--)
            yield return i;
    }
}
#endregion