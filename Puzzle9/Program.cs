using Common;

//part1("test.txt");
part1("input.txt");
//part2("test.txt");
part2("input.txt");

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
    var poly = new Polygon2D(vectors);
    
    // create areas and sort by size descending
    // select first area fully contained in polygon
    var max = vectors.CreateTuple2()
        .Select(x => new VectorPlane2D(x.Item1, x.Item2))
        .OrderByDescending(a => a.Size)
        .FirstOrDefault(poly.ContainsRectangleInclusive);

    Console.WriteLine($"max area {max?.Size}");
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
        .Select(v => new Vector2D(v[1], v[0]))
        .ToList();
    var max = vectors.CreateTuple2()
        .Select(x => new VectorPlane2D(x.Item1, x.Item2))
        .Max(x => x.Size);

    Console.WriteLine($"max {max}");
}
#endregion