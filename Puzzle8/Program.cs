using Common;
using System.Diagnostics;

part1("test.txt", 10);
part1("input.txt", 1000);

void part1(string file, int rounds) {
    var vectors = File.ReadAllLines(file)
        .Select(line => line
            .Split(',')
            .Select(long.Parse).ToArray()
        )
        .Select(v => new Vector3D(v[0], v[1], v[2]))
        .ToList();

    var relations = new SortedDictionary<double, HashSet<(Vector3D, Vector3D)>>();
    for (var i = 0; i < vectors.Count; i++) {
        for (var j = i + 1; j < vectors.Count; j++) {
            var a = vectors[i];
            var b = vectors[j];

            var key = Distance(a, b);
            relations.AddValue(key, (a, b));
        }
    }

    var allJunction = vectors.Select(x => new HashSet<Vector3D>{ x }).ToList();
    HashSet<Vector3D> connected = new();
    foreach (var (distance, pairs) in relations.Take(rounds)) {
        foreach (var pair in pairs) {
            var foundJunctions = allJunction.FindAll(
                x => (x.Contains(pair.Item1) || x.Contains(pair.Item2)))
                .ToList();
            Debug.Assert(foundJunctions.Count >= 0);
            Debug.Assert(foundJunctions.Count <= 2);

            HashSet<Vector3D>? updateJunction = foundJunctions.First();
            // add current elements
            updateJunction.Add(pair.Item1);
            updateJunction.Add(pair.Item2);
            // merge sets
            foundJunctions.Skip(1).ToList().ForEach(x => updateJunction.UnionWith(x));
            foundJunctions.Skip(1).ToList().ForEach(x => allJunction.Remove(x));

            // remember visited
            connected.Add(pair.Item1);
            connected.Add(pair.Item2);
        }
    }

    var result = allJunction
        .Select(x => x.Count)
        .OrderDescending()
        .Take(3)
        .Aggregate((a, b) => a * b);
    Console.WriteLine($"sum {result}");

}

double Distance(Vector3D a, Vector3D b) {
    // return Math.Sqrt(
    //     Math.Pow(a.X - b.X, 2)
    //     + Math.Pow(a.Y - b.Y, 2)
    //     + Math.Pow(a.Z - b.Z, 2)
    // );
    var dx = a.X - b.X;
    var dy = a.Y - b.Y;
    var dz = a.Z - b.Z;
    return (dx * dx + dy * dy + dz * dz);
}
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

public static class DictionaryExtensions {
    public static void AddValue<TKey, TValue>(this IDictionary<TKey, HashSet<TValue>> d, TKey k, TValue v) {
        if (!d.TryGetValue(k, out var elements)) {
            d[k] = elements = new();
        }
        elements.Add(v);
    }
}