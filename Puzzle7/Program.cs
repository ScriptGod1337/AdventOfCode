using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string pattern = @"^(\d+):\s((?:\d+\s?)*)$";

long sum = 0;
foreach (var line in File.ReadLines("input.txt")) {
    var matches = Regex.Match(line, pattern);

    var result = long.Parse(matches.Groups[1].Value);
    var input = matches.Groups[2].Value.Split(' ').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => long.Parse(x)).ToList();

    if (calculateCombinations(input).Contains(result)) {
        sum += result;
    }
}
Console.WriteLine(sum);

List<long> calculateCombinations(List<long> numbers) {
    var results = new List<long>();
    compute(numbers, 0, numbers[0], results);
    return results;
}

void compute(List<long> numbers, int idx, long current, List<long> results) {
    if (idx == numbers.Count - 1) {
        results.Add(current);
        return;
    }

    // Get the next number in the array
    var nextNumber = numbers[idx + 1];

    // Add the next number
    compute(numbers, idx + 1, current + nextNumber, results);

    // Multiply by the next number
    compute(numbers, idx + 1, current * nextNumber, results);

    // Concatenate by the next number
    var combined = long.Parse($"{current}{nextNumber}");
    compute(numbers, idx + 1, combined, results);
}
