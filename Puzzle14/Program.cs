using System.Text.RegularExpressions;

var area = new Vector(101, 103);
var areaMid = area / 2;

var robots = parseInput();
//part1();
part2();

void part2() {
    var robots2 = robots.Select(x => new Robot(x.Item1, x.Item2, area)).ToList();

    var writer = new StreamWriter(Console.OpenStandardOutput());
    
    var minVariance = double.MaxValue;
    var minCount = -1;
    Dictionary<Vector, long> minPos = null;
    for (int n = 0; n < 10_000; n++) {
        if (n % 10_000 == 0) {
            GC.Collect();
        }

        var allPos = new Dictionary<Vector, long>(robots2.Count);
        robots2.ForEach(x => {
            var pos = x.step();
            if (!allPos.TryAdd(pos, 1)) {
                allPos[pos]++;
            }
        });
        double varianceX = CalculateVariance(allPos.Keys.Select(v => v.x));
        double varianceY = CalculateVariance(allPos.Keys.Select(v => v.y));
        var variance = varianceX + varianceY;
        if (variance < minVariance) {
            Console.WriteLine($"{n} - {minVariance} -> {variance}");
            minVariance = variance;
            minCount = n;
            minPos = allPos;
        }
    }

    Console.WriteLine($"{minCount} = {minVariance}");
    render(minCount, writer, minPos, area);
}

void render(int num, StreamWriter writer, Dictionary<Vector, long> pos, Vector area) {
    writer.WriteLine(num);
    writer.WriteLine("----------------");
    for (int i = 0; i < area.x; i++) {
        for (int j = 0; j < area.y; j++) {
            long count;
            if (pos.TryGetValue(new Vector(i, j), out count)) {
                writer.Write(count);
            } else {
                writer.Write(".");
            }

        }
        writer.WriteLine();
    }
    writer.WriteLine("----------------");
}

double CalculateVariance(IEnumerable<long> values) {
    // Calculate mean
    double mean = values.Average();

    // Calculate the sum of squared differences from the mean
    double squaredDifferencesSum = values.Sum(value => Math.Pow(value - mean, 2));

    // Divide by the number of elements to get population variance
    return squaredDifferencesSum / values.Count();
}

#region part
#if false
void part1() {
    var steps = 100;

    int[,] count = new int[2, 2];
    foreach (var (p, v) in robots) {
        var newPosRaw = p + (v * steps);
        var newPos = wrapArea(newPosRaw, area);

        if ((newPos.x == areaMid.x) || (newPos.y == areaMid.y)) {
            Console.WriteLine($"{p.x} {p.y} -> {newPos.x} {newPos.y} skipped");
            continue;
        }

        var gridX = ((newPos.x < areaMid.x) ? 0 : 1);
        var gridY = ((newPos.y < areaMid.y) ? 0 : 1);
        count[gridX, gridY]++;
        Console.WriteLine($"{p.x} {p.y} -> {newPos.x} {newPos.y} / {gridX} {gridY}={count[gridX, gridY]}");
    }

    Console.WriteLine($"{count[0, 0]} {count[1, 0]} {count[0, 1]} {count[1, 1]}");
    var sum = count[0, 0] * count[1, 0] * count[0, 1] * count[1, 1];
    Console.WriteLine(sum);
}

Vector wrapArea(Vector p, Vector area) {
    p %= area;

    return new Vector((p.x < 0) ? area.x + p.x : p.x, //
        (p.y < 0) ? area.y + p.y : p.y);
}
#endif
#endregion

List<(Vector, Vector)> parseInput() {
    var robots = new List<(Vector, Vector)>();

    foreach (var line in File.ReadLines("input.txt")) {
        var match = Regex.Match(line, @"p=(-?\d+),(-?\d+)\s+v=(-?\d+),(-?\d+)");
        if (!match.Success) {
            continue;
        }

        int px = int.Parse(match.Groups[1].Value);
        int py = int.Parse(match.Groups[2].Value);
        int vx = int.Parse(match.Groups[3].Value);
        int vy = int.Parse(match.Groups[4].Value);
        robots.Add((new Vector(px, py), new Vector(vx, vy)));
    }

    return robots;
}

class Robot {

    private Vector pos;
    private Vector v;

    private Vector area;

    public Robot(Vector pos, Vector v, Vector area) {
        this.pos = pos;
        this.v = v;
        this.area = area;
    }

    public Vector step() {
        pos = wrapArea(pos + v, area);
        return pos;
    }

    private Vector wrapArea(Vector p, Vector area) {
        p %= area;

        return new Vector((p.x < 0) ? area.x + p.x : p.x, //
            (p.y < 0) ? area.y + p.y : p.y);
    }
}

record Vector(long x, long y) {
    public static Vector operator +(Vector p1, Vector p2) => new Vector(p1.x + p2.x, p1.y + p2.y);
    public static Vector operator -(Vector p1, Vector p2) => new Vector(p1.x - p2.x, p1.y - p2.y);
    public static Vector operator %(Vector p1, Vector p2) => new Vector(p1.x % p2.x, p1.y % p2.y);
    public static Vector operator *(Vector p, long factor) => new Vector(p.x * factor, p.y * factor);
    public static Vector operator *(long factor, Vector p) => new Vector(p.x * factor, p.y * factor);
    public static Vector operator /(Vector p, long factor) => new Vector(p.x / factor, p.y / factor);
    public static Vector operator /(long factor, Vector p) => new Vector(p.x / factor, p.y / factor);
}