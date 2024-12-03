using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World3!");

string filePath = "input.txt";

Regex regexMul = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");

//part1();
part2();

void part1() {
    int sum = 0;
    foreach (string line in File.ReadLines(filePath)) {
        var matchesMul = regexMul.Matches(line);

        foreach (Match match in matchesMul) {
            // Extract captured groups (x and y)
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);

            sum += (x * y);
        }
    }
    Console.WriteLine(sum);
}

void part2() {
    var fullContent = string.Join("", File.ReadLines(filePath));

    var matchesMul = regexMul.Matches(fullContent);
    var matchesOnOff = new Regex(@"(?:do\(\)|don't\(\))").Matches(fullContent);
    var onOffLookup = new SortedDictionary<int, bool>(matchesOnOff.ToDictionary(
        matches => matches.Index,
        matches => matches.Value.Equals("do()"))
    );
    onOffLookup[0] = true;

    int sum = 0;
    foreach (Match match in matchesMul) {
        // Extract captured groups (x and y)
        int x = int.Parse(match.Groups[1].Value);
        int y = int.Parse(match.Groups[2].Value);

        if (onOffLookup.Last(x => x.Key < match.Index).Value) {
            sum += (x * y);
        }
    }
    Console.WriteLine(sum);
}