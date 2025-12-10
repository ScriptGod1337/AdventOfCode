using Common;

part1("test.txt");
part1("input.txt");

#region part2
void part2(string file) {
    var vectors = File.ReadAllLines(file)
        .Select(line => line
            .Split(',')
            .Select(long.Parse)
            .ToArray()
        )
        .Select(v => new Vector2D(v[0], v[1]))
        .ToList();
    
}
#endregion

#region part1
void part1(string file) {
    var vectors = File.ReadAllLines(file)
        .Select(line => line
            .Split(',')
            .Select(long.Parse)
            .ToArray()
        )
        .Select(v => new Vector2D(v[0], v[1]))
        .ToList();
    var areas = vectors.CreateTuple2()
        .Select(x => new VectorPlane2D(x.Item1, x.Item2));

    var max = areas
        .Select(x => x.Size)
        .Max();
    Console.WriteLine($"maxArea {max}");
}
#endregion