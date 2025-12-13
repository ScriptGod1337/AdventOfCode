using Common;

#region external

// using Advent.Common;

// var points = File.ReadAllLines("test.txt")
//     .Select(line => line
//         .Split(',')
//         .Select(long.Parse)
//         .ToArray()
//     )
//     .Select(v => new Pos((int)v[0], (int)v[1]))
//     .ToArray();
// var edges = points.Append(points[0])
//     .Chain()
//     .Select(p => new Line(p.First, p.Second))
//     .ToArray();
// 
// var x = points.EnumeratePairs()
//     .Select(p => new Rect(p.First, p.Second))
//     .Where(rect => !edges.Any(edge => rect.Intersects(edge)))
//     .Where(rect => IsInside(edges, rect.From))
//     //.Where(area => area.Area == 24)
//     //.Max(a => a.Area);
//     .OrderBy(a => a.Area)
//     .ToList();
// x.ForEach(x => Console.WriteLine($"Size {x.Area}\t{x}"));
// Console.WriteLine($"count {x.Count}");
// Console.WriteLine($"max {x.Max(a => a.Area)}");

// static bool IsInside(Line[] edges, Pos p)
//     => edges.Count(a => a.IsVertical && a.Start.X > p.X && a.Min.Y <= p.Y && a.Max.Y > p.Y) % 2 == 1;
#endregion

#region external2
    static Dictionary<Tuple<Point, Point>, long> FindLargestSquarePart1(Point[] tiles)
    {
        Dictionary<Tuple<Point, Point>, long> squares = [];
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            for (int n = 1; n < tiles.Length; n++)
            {
                var p1 = tiles[i];
                var p2 = tiles[n];

                var area = (Math.Abs(p1.X - p2.X) + 1L) * (Math.Abs(p1.Y - p2.Y) + 1L);
                squares.Add(new Tuple<Point, Point>(p1, p2), area);
            }
        }
        return squares;
    }

    static long FindLargestSquarePart2(Point[] tiles, Dictionary<Tuple<Point, Point>, long> squares)
    {
        var edges = GetEdges(tiles);

        foreach (var square in squares)
        {
            var p1 = square.Key.Item1;
            var p2 = square.Key.Item2;

            var testPoint1 = new Point(p1.X, p2.Y);
            var testPoint2 = new Point(p2.X, p1.Y);

            if(!TestSquare([p1, p2], edges)) continue;

            return square.Value;
            // var area = (Math.Abs(p1.X - p2.X) + 1L) * (Math.Abs(p1.Y - p2.Y) + 1L);
            // return area;
        }
        return 0L;
    }

    static bool TestSquare(Point[] square, HashSet<Point> edges)
    {
        for (int i = 0; i < square.Length; i++)
        {
            var t1 = square[i];
            var t2 = square[(i + 1) % square.Length];

            var minY = Math.Min(t1.Y, t2.Y);
            var maxY = Math.Max(t1.Y, t2.Y);
            var minX = Math.Min(t1.X, t2.X);
            var maxX = Math.Max(t1.X, t2.X);

            for(int x = minX + 1; x < maxX; x++)
            {
                if(edges.Contains(new Point(x, minY + 1)) || edges.Contains(new Point(x, maxY - 1)))
                    return false;
            }
            for(int y = minY + 1; y < maxY; y++)
            {
                if(edges.Contains(new Point(minX + 1, y)) || edges.Contains(new Point(maxX - 1, y)))
                    return false;
            }
        }
        return true;
    }

    static HashSet<Point> GetEdges(Point[] tiles)
    {
        HashSet<Point> edges = new HashSet<Point>();
        for (int i = 0; i < tiles.Length; i++)
        {
            var t1 = tiles[i];
            var t2 = tiles[(i + 1) % tiles.Length];
            if (t1.X == t2.X)
            {
                var minY = Math.Min(t1.Y, t2.Y);
                var maxY = Math.Max(t1.Y, t2.Y);

                for (int y = minY; y <= maxY; y++)
                {
                    edges.Add(new Point(t1.X, y));
                }
            }
            else if (t1.Y == t2.Y)
            {
                var minX = Math.Min(t1.X, t2.X);
                var maxX = Math.Max(t1.X, t2.X);

                for (int x = minX; x <= maxX; x++)
                {
                    edges.Add(new Point(x, t1.Y));
                }
            }
        }
        return edges;
    }

    static Point[] ParseInput(string[] input)
    {
        var tiles = new List<Point>();
        foreach (var line in input)
        {
            var parts = line.Split(",");
            tiles.Add(new(int.Parse(parts[0]), int.Parse(parts[1])));
        }
        return [.. tiles];
    }

// var tiles = ParseInput( File.ReadAllLines("input.txt") );;
// var squareAreas = FindLargestSquarePart1(tiles);
// squareAreas = squareAreas.OrderByDescending(c => c.Value).ToDictionary();
// FindLargestSquarePart2(tiles, squareAreas);
#endregion

Console.WriteLine($"--- part 1 ---");

//part1("test.txt");
//part1("input.txt");
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
    var test = poly.ContainsRectangle(new VectorPlane2D(new Vector2D(7, 3), new Vector2D(11, 1)));
    
    var areasSorted = vectors.CreateTuple2()
        .Select(x => new VectorPlane2D(x.Item1, x.Item2))
        .OrderByDescending(a => a.Size);

    var max = areasSorted
        // .Where(area => !area.AllCorners.Any(
        //         corner => !poly.Contains(corner)
        //     )
        // )
        .Where(area => poly.ContainsRectangle(area))
        .FirstOrDefault();
    Console.WriteLine($"max area {max?.Size}");

    //     //.Where(area => area.Size == 24)
    //     .OrderBy(a => a.Size)
    //     .ToList();
    // max.ForEach(x => Console.WriteLine($"Size {x.Size}\t{x}"));
    // Console.WriteLine($"count {max.Count}");
    // Console.WriteLine($"max {max.Max(a => a.Size)}");

    // using (StreamWriter writer = new StreamWriter(/*Console.OpenStandardOutput()*/"output.txt")) {
    //     printWorld(vectors.ToHashSet(), areasSorted, poly, writer);
    // }
}

void printWorld(ISet<Vector2D> vectors, IList<VectorPlane2D>? areas, Polygon2D? poly, StreamWriter writer) {
    var maxX = vectors.Select(v => v.X).Max();
    var maxY = vectors.Select(v => v.Y).Max();
    for (var row = 0; row <= maxY + 1; row++) {
        for (var col = 0; col <= maxX + 1; col++) {
            if (vectors.Contains(new Vector2D(col, row))) {
                if ((poly != null)
                    && poly.ContainsPoint(new Vector2D(col, row))) {
                    writer.Write("+");
                } else 
                    writer.Write("#");
            } else if ((areas != null)
                && areas.Any(area => area.Contains(new Vector2D(col, row)))) {
                writer.Write("O");
            } else if ((poly != null)
                && poly.ContainsPoint(new Vector2D(col, row))) {
                writer.Write("O");
            } else {
                writer.Write(".");
            }
        }

        writer.WriteLine();
    }
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
    var areas = vectors.CreateTuple2()
        .Select(x => new VectorPlane2D(x.Item1, x.Item2));

    var max = areas
        .Select(x => x.Size)
        .Max();
    Console.WriteLine($"max {max}");
}
#endregion

record Point(int X, int Y);