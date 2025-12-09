
using System.Diagnostics;
using Common;

part("input.txt");

void part(string file) {
    var vectors = File.ReadAllLines(file)
        .Select(line => line
            .Split(',')
            .Select(long.Parse)
            .ToArray()
        )
        .Select(v => new Vector2D(v[0], v[1]))
        .ToList();

    var sorted = vectors
        .OrderBy(v => v.X)
        .ThenBy(v => v.Y)
        .ToList();
    var pairs = new List<(Vector2D, Vector2D)>();
    for (var i = 0; i < sorted.Count; i++) {
        for (var j = i + 1; j < sorted.Count; j++) {
            pairs.Add((vectors[i], vectors[j]));
        }
    }

    long maxArea = 0;
    foreach (var (a, b) in pairs) {
        var area = Area(a, b);
        // Console.WriteLine($"{a} x {b} = {area}");
        if (area > maxArea) {
            maxArea = area;
        }
    }

    Console.WriteLine($"maxArea {maxArea}");
}

static long Area(Vector2D a, Vector2D b) =>
    (Math.Abs(a.X - b.X) + 1) * (Math.Abs(a.Y - b.Y) + 1);