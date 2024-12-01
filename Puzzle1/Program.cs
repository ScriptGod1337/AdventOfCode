using System;
using System.IO;
using System.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string filePath = "input.txt";

var combinedMap = new Dictionary<string, String>
{
    // Word to Digit mappings
    { "one", "1" },
    { "two", "2" },
    { "three", "3" },
    { "four", "4" },
    { "five", "5" },
    { "six", "6" },
    { "seven", "7" },
    { "eight", "8" },
    { "nine", "9" },

    // Digit to Number mappings
    { "1", "1" },
    { "2", "2" },
    { "3", "3" },
    { "4", "4" },
    { "5", "5" },
    { "6", "6" },
    { "7", "7" },
    { "8", "8" },
    { "9", "9" }
};
// List<string> combinedList = new List<string>(digitToNumberMap.Keys);
// combinedList.AddRange(wordToDigitMap.Keys);

// foreach (var obj in test) {
//     Console.WriteLine(string.Join(",", obj));
// }

var test1 = File.ReadLines(filePath).Select(line => //
        line.Where(c => char.IsDigit(c))).Select(chars => //
            int.Parse(chars.First().ToString() + chars.Last().ToString())
    ).Sum();
Console.WriteLine(test1);

// foreach (var obj in File.ReadLines(filePath)) {

// }

var test2 = File.ReadLines(filePath).Select(line => sumUp(line)).Sum();
Console.WriteLine(test2);


int sumUp(string line) {
    var allMatches = matches(line, combinedMap.Keys);
    var firstMatch = allMatches.Keys.Min();
    var lastMatch = allMatches.Keys.Max();

    var first = combinedMap[allMatches[firstMatch]];
    var last = combinedMap[allMatches[lastMatch]];
    var number = first + last;
    return int.Parse(number);
}

Dictionary<int, String> matches(string target, IEnumerable<String> allSearch) {

    Dictionary<int, String> result = new Dictionary<int, string>();

    foreach (var search in allSearch) {
        var matches = FindAllOccurrences(target, search);
        matches.ForEach(c => result[c] = search);
    }

    return result;
}

List<int> FindAllOccurrences(string target, string search)
{
    List<int> positions = new List<int>();
    int position = target.IndexOf(search, 0, StringComparison.OrdinalIgnoreCase); // Start from position 0

    // Iterate and find all occurrences
    while (position != -1)
    {
        positions.Add(position);  // Add current position to the list
        position = target.IndexOf(search, position + 1, StringComparison.OrdinalIgnoreCase);  // Find next occurrence
    }

    return positions;
}