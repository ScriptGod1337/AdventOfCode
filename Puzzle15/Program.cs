using System.Diagnostics;
using System.Runtime.CompilerServices;

(World world, List<Direction> directions) = parseInput("input.txt");
// world.Render(14, 7);
// Console.WriteLine();
world.Walk(directions);
var result = world.calcGPSAll();
Console.WriteLine(result);

(World, List<Direction>) parseInput(string file) {
    var inputLines = File.ReadAllLines(file);

    var lines = inputLines.Select(line => {
        return string.Join("", line.Select(c => {
            string newChar;
            switch (c) {
                case '.':
                    newChar = "..";
                    break;
                case '#':
                    newChar = "##";
                    break;
                case '@':
                    newChar = "@.";
                    break;
                case 'O':
                    newChar = "[]";
                    break;
                default:
                    newChar = c.ToString();
                    break;
            }
            return newChar;
        }));
    }).ToList();

    Vector guard = null;
    var boxes = new List<Box>();
    var walls = new List<Vector>();
    var walks = new List<Direction>();
    for (var y = 0; y < lines.Count; y++) {
        var line = lines[y];
        if (string.IsNullOrWhiteSpace(line)) {
            continue;
        }

        for (var x = 0; x < line.Length; x++) {
            var c = line[x];
            switch (c) {
                case '.':
                    continue;
                case '@':
                    guard = new Vector(x, y);
                    break;
                case '[':
                    boxes.Add(new Box(new Vector(x, y)));
                    break;
                case ']':
                    break;
                case '#':
                    walls.Add(new Vector(x, y));
                    break;
                default:
                    walks.Add(new Direction(c));
                    break;
            }
        }
    }

    return (new World(boxes, walls, guard), walks);
}

class World {
    private List<Box> boxes;
    private List<Vector> walls;
    private Vector guard;

    public World(List<Box> boxes, List<Vector> walls, Vector guard) {
        this.boxes = boxes;
        this.walls = walls;
        this.guard = guard;
    }

    private Box? getBox(Vector v) {
        return boxes.Find(x => x.IsHit(v));
    }

    public void Walk(List<Direction> walks) {
        foreach (var step in walks) {
            // Console.WriteLine($"{step.Step.x} {step.Step.y}");

            var next = guard + step.Step;
            Box? box;
            if (walls.Contains(next)) {
                continue;
            } else if ((box = getBox(next)) != null) {
                if (MoveBoxes(box, step.Step)) {
                    guard = next; // boxes moved
                }
            } else {
                guard = next; // no obstacle
            }

            // Render(14, 7);
            // Console.WriteLine();
        }

    }

    private bool MoveBoxes(Box box, Vector step) {
        var visitedBoxes = new HashSet<Box>();
        if (CanBoxesMoved(box, step, visitedBoxes)) {
            visitedBoxes.ToList().ForEach(x => x.MoveBy(step)); // moved all boxes
            return true;
        } else {
            return false; // not possible
        }
    }

    private bool CanBoxesMoved(Box box, Vector step, ICollection<Box> visitedBoxes) {
        visitedBoxes.Add(box);
        var allNextPos = box.passBy(step);
        foreach (var nextPos in allNextPos) {
            if (!CanBoxesMoved(step, visitedBoxes, nextPos)) {
                return false; // not possible
            }
        }

        return true; // all possible
    }

    private bool CanBoxesMoved(Vector step, ICollection<Box> visitedBoxes, Vector nextPos) {
        if (walls.Contains(nextPos)) {
            return false; // can't move
        }

        Box? nextBox;
        if ((nextBox = getBox(nextPos)) != null) {
            return CanBoxesMoved(nextBox, step, visitedBoxes);
        } else {
            return true; // can be moved
        }
    }

    public long calcGPSAll() {
        return boxes.Select(x => x.CalcGPS()).Sum();
    }

    public void Render(int sizeX, int sizeY) {
        for (int y = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                var v = new Vector(x, y);
                if (walls.Contains(v)) {
                    Console.Write("#");
                } else if (getBox(v) != null) {
                    Console.Write("O");
                } else if (v.Equals(guard)) {
                    Console.Write("@");
                } else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }
}

class Box {
    private Vector left;
    private Vector right;

    public Box(Vector left) {
        this.left = left;
        this.right = left + new Vector(1, 0);
    }

    public bool IsHit(Vector v) {
        return (left == v) || (right == v);
    }

    public void MoveBy(Vector v) {
        left += v;
        right += v;
    }

    public IEnumerable<Vector> passBy(Vector v) {
        Debug.Assert((v.x == 0) ^ (v.y == 0));
        if (v.x != 0) {
            return [(v.x < 0) ? left + v : right + v];
        } else {
            return [left + v, right + v];
        }
    }

    public long CalcGPS() {
        return left.x + left.y * 100;
    }
}

#region part1
#if false
(World, List<Direction>) parseInput(string file) {
    var lines = File.ReadAllLines(file);

    Vector guard = null;
    var boxes = new List<Vector>();
    var walls = new List<Vector>();
    var walks = new List<Direction>();
    for (var y = 0; y < lines.Length; y++) {
        var line = lines[y];
        if (string.IsNullOrWhiteSpace(line)) {
            continue;
        }

        for (var x = 0; x < line.Length; x++) {
            var c = line[x];
            switch (c) {
                case '.':
                    continue;
                case '@':
                    guard = new Vector(x, y);
                    break;
                case 'O':
                    boxes.Add(new Vector(x, y));
                    break;
                case '#':
                    walls.Add(new Vector(x, y));
                    break;
                default:
                    walks.Add(new Direction(c));
                    break;
            }
        }
    }

    return (new World(boxes, walls, guard), walks);
}

class World {
    private List<Vector> boxes;
    private List<Vector> walls;
    private Vector guard;

    public World(List<Vector> boxes, List<Vector> walls, Vector guard) {
        this.boxes = boxes;
        this.walls = walls;
        this.guard = guard;
    }

    public void Walk(List<Direction> walks) {
        foreach (var step in walks) {
            // Console.WriteLine($"{step.Step.x} {step.Step.y}");
            var next = guard + step.Step;
            if (walls.Contains(next)) {
                continue;
            } else if (boxes.Contains(next)) {
                if (ShiftBoxes(next, step.Step)) {
                    guard = next; // boxes moved
                }
            } else {
                guard = next; // no obstacle
            }

            // Render(8, 8);
            // Console.WriteLine();
        }
        
    }

    private bool ShiftBoxes(Vector box, Vector direction) {
        var next = box + direction;
        if (walls.Contains(next)) {
            return false; // can't be shifted
        }
        
        bool canMove;
        if (boxes.Contains(next)) {
            canMove = ShiftBoxes(next, direction);
        } else {
            canMove = true; // space
        }

        if (canMove) {
            boxes.Remove(box);
            boxes.Add(next);
        }

        return canMove;
    }

    public long calcGPSAll() {
        return boxes.Select(x => calcGPSBox(x)).Sum();
    }

    public long calcGPSBox(Vector box) {
        return box.x + box.y * 100;
    }

    public void Render(int sizeX, int sizeY) {
        for (int y = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                var v = new Vector(x, y);
                if (walls.Contains(v)) {
                    Console.Write("#");
                } else if (boxes.Contains(v)) {
                    Console.Write("O");
                } else if (v.Equals(guard)) {
                    Console.Write("@");
                }else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }
}
#endif
#endregion

class Direction {

    private static readonly Dictionary<char, Vector> ALL_STEPS = new Dictionary<char, Vector> {
        { '<', new Vector(-1, 0) },
        { '>', new Vector(1, 0) },
        { '^', new Vector(0, -1) },
        { 'v', new Vector(0, 1) },
    };

    public Vector Step { get; init; }

    public Direction(char c) {
        Step = ALL_STEPS[c];
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