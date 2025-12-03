var batteries = new List<List<int>>();

foreach (var line in File.ReadLines("input.txt")) {
    var nums = line
        .Where(char.IsDigit)
        .Select(c => c - '0')
        .ToList();

    batteries.Add(nums);
}

part2();

#region part2
void part2() {
    Console.WriteLine($"sum {batteries.Select(digits => toNumber(maxSubSeq(digits, 12))).Sum()}");
}

long toNumber(List<int> digits) {
    long result = 0;
    foreach (var d in digits) {
        result = result * 10 + d;
    }

    return result;
} 

List<int> maxSubSeq(IList<int> digits, int k) {
    int remove = digits.Count - k;
    var stack = new List<int>(k);

    foreach (var d in digits) {
        while ((remove > 0) && (stack.Count > 0 && stack[^1] < d)) {
            stack.RemoveAt(stack.Count - 1);
            remove--;
        }
        stack.Add(d);
    }

    return stack.GetRange(0, k);
}
#endregion

#region part1
void part1() {
    Console.WriteLine($"sum {batteries.Select(toHighNumber).Sum()}");
}

int toHighNumber(List<int> digits) {
    List<int> buildNumbers = new List<int>();

    for (var i = 0; i < digits.Count; i++) {
        for (var j = i + 1; j < digits.Count; j++) {
            buildNumbers.Add(digits[i] * 10 + digits[j]);
        }

    }

    return buildNumbers.OrderDescending().First();
}
#endregion