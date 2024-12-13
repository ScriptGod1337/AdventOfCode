using System.Collections.Immutable;
using System.ComponentModel;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var world = new World("input.txt");
var proce = new WorldProcesor(world);
proce.process();

class WorldProcesor {

    private static List<Position> steps = new List<Position>() {
        new Position(0, -1), // left
        new Position(0, 1), // right
        new Position(1, 0), // down
        new Position(-1, 0), // up
    };

    private World world;

    public WorldProcesor(World world) {
        this.world = world;
    }

    // part 2
    #region part2
    private HashSet<Position> visitedGlobal = new HashSet<Position>();

    public void process() {
        var dim = world.GetDimension();

        var result = 0;
        for (int i = 0; i < dim.row; i++) {
            for (int j = 0; j < dim.col; j++) {
                var visitedBefore = visitedGlobal.Count;
                var walls = process(new Position(i, j));
                var area = visitedGlobal.Count- visitedBefore;
                if (area == 0) {
                    continue;
                }
                var changes = countDirectionChanges(walls);
                
                Console.WriteLine($"{i}{j} {world.GetAt(i, j)} {area} {walls.ToList().Count} {changes}");
                result += changes * area;
            }
        }

        Console.WriteLine(result);
    }

    private int countDirectionChanges(ICollection<(Position, Position)> walls) {
        var changes = 0;
        var toProcess = walls.OrderBy(x => x.Item1.row).ThenBy(x => x.Item1.col).ToList();
        while (toProcess.Count > 0) {
            changes++;
            var current = toProcess.First();
            toProcess.Remove(current);
            
            var col = current.Item1.col;
            while (true) {
                var next = new Position(current.Item1.row, ++col);
                if (!walls.Contains((next, current.Item2))) {
                    break;
                }

                toProcess.Remove((next, current.Item2));
            }

            var row = current.Item1.row;
            while (true) {
                var next = new Position(++row, current.Item1.col);
                if (!walls.Contains((next, current.Item2))) {
                    break;
                }

                toProcess.Remove((next, current.Item2));
            }
        }

        return changes;
    }

    private ICollection<(Position, Position)> process(Position pos) {
        if (visitedGlobal.Contains(pos)) {
            return ImmutableHashSet<(Position, Position)>.Empty; // already processed
        }
        visitedGlobal.Add(pos); // visited

        var walls = new HashSet<(Position, Position)>();
        steps.ForEach(direction => walls.UnionWith(process(pos, direction)));
        return walls;
    }

    private ICollection<(Position, Position)> process(Position pos, Position direction) {
        var value = world.GetAt(pos);

        var walls = new HashSet<(Position, Position)>();
        var nextPos = pos + direction;
        var nextValue = world.GetAt(nextPos);
        if (nextValue.HasValue && nextValue.Value.Equals(value.Value)) {
            walls.UnionWith(process(nextPos));
        } else {
            walls = new HashSet<(Position, Position)>() { (pos, direction) };
        }

        return walls;
    }
    #endregion

    // part 1
    #region part1
#if false
    private HashSet<Position> visitedGlobal = new HashSet<Position>();

    public void process() {
        var dim = world.GetDimension();

        var result = 0;
        for (int i = 0; i < dim.row; i++) {
            for (int j = 0; j < dim.col; j++) {
                var visitedBefore = visitedGlobal.Count;
                var walls = process(new Position(i, j));
                if (walls == 0) {
                    continue;
                }
                
                Console.WriteLine($"{i}{j} {world.GetAt(i, j)} {walls} {visitedGlobal.Count- visitedBefore}");
                result += walls * (visitedGlobal.Count- visitedBefore);
            }
        }

        Console.WriteLine(result);
    }

    public int process(Position pos) {
        if (visitedGlobal.Contains(pos)) {
            return 0; // already processed
        }
        visitedGlobal.Add(pos); // visited

        return steps.Sum(x => process(pos, x));
    }

    public int process(Position pos, Position direction) {
        var value = world.GetAt(pos);

        int walls;
        var nextPos = pos + direction;
        var nextValue = world.GetAt(nextPos);
        if (nextValue.HasValue && nextValue.Value.Equals(value.Value)) {
            walls = process(nextPos);
        } else {
            walls = 1;
        }

        return walls;
    }
#endif
    #endregion
}
class World {
    private char[,] matrix;

    public World(string filePath) {
        // Read all lines from the file
        string[] lines = File.ReadAllLines(filePath);

        // Determine the dimensions of the matrix
        int rows = lines.Length;
        int cols = lines[0].Length;

        // Create the matrix
        matrix = new char[rows, cols];

        // Fill the matrix
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                matrix[i, j] = lines[i][j];
            }
        }
    }

    public Position GetDimension() {
        return new Position(matrix.GetLength(0), matrix.GetLength(1));
    }

    public char? GetAt(Position pos) {
        return GetAt(pos.row, pos.col);
    }

    public char? GetAt(int row, int col) {
        if (IsOutOfArea(row, col)) {
            return null;
        }

        return matrix[row, col];
    }

    public bool IsOutOfArea(Position pos) {
        return IsOutOfArea(pos.row, pos.col);
    }

    public bool IsOutOfArea(int row, int col) {
        // Check if the position is out of bounds
        if (row < 0 || col < 0 || row >= matrix.GetLength(0) || col >= matrix.GetLength(1)) {
            return true; // Out of bounds
        } else {
            return false;
        }
    }
}

record Position(int row, int col) {
    public static Position operator +(Position p1, Position p2) => new Position(p1.row + p2.row, p1.col + p2.col);
    public static Position operator -(Position p1, Position p2) => new Position(p1.row - p2.row, p1.col - p2.col);
}