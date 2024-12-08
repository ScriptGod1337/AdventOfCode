using System.Collections.Specialized;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var (world, guard) = ParseFile("input.txt");
part2();

void part2() {
    var data = world.data;
    int rows = data.GetLength(0);
    int cols = data.GetLength(1);
    int loops = 0;

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {

            var pos = new Position(i, j);
            if (world.IsBlocked(pos) || guard.Position.Equals(pos)) {
                continue;
            }

            data[i, j] = true;
            if (isLoop(rows, cols)) {
                loops++;
            }
            data[i, j] = false;
        }
    }

    Console.WriteLine(loops);
}

bool isLoop(int rows, int cols) {
    var startPos = guard.Position;
    var startDir = guard.Direction;

    var visited = new HashSet<Position>();
    visited.Add(guard.Position);

    var stepCount = 0;
    var inLoop = false;
    var stop = false;
    while (!stop) {
        var newPos = guard.Position + guard.Direction.Step;
        if (world.IsOutOfArea(newPos)) {
            //world.Print(guard, visited);
            stop = true;
        } else if (world.IsBlocked(newPos)) {
            //world.Print(guard, visited);
            guard.Turn();
        } else if (stepCount > (rows * cols)) {
            stop = true;
            inLoop = true;
        } else {
            guard.Position = newPos;
            stepCount++;
            visited.Add(newPos);
        }
    }

    guard.Position = startPos;
    guard.Direction = startDir;

    return inLoop;
}

void part1() {
    var visited = new HashSet<Position>();
    visited.Add(guard.Position);
    int turns = 0;
    var stop = false;
    while (!stop) {
        var newPos = guard.Position + guard.Direction.Step;
        if (world.IsOutOfArea(newPos)) {
            world.Print(guard, visited);
            stop = true;
        } else if (world.IsBlocked(newPos)) {
            world.Print(guard, visited);
            guard.Turn();
            turns++;
        } else {
            guard.Position = newPos;
            visited.Add(newPos);
        }
    }
    Console.WriteLine(visited.Count);
}

(World, Guard) ParseFile(string filePath) {
    // Read all lines from the file
    string[] lines = File.ReadAllLines(filePath);

    Guard guard = null;

    // Determine the dimensions of the matrix
    int rows = lines.Length;
    int cols = lines[0].Length;

    // Create the matrix
    bool[,] matrix = new bool[rows, cols];

    // Fill the matrix
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            matrix[i, j] = lines[i][j] == '#';
            if (lines[i][j] != '#' && lines[i][j] != '.') {
                guard = new Guard(lines[i][j], new Position(i, j));
            }
        }
    }

    return (new World(matrix), guard);
}

class World {
    public bool[,] data;

    public World(bool[,] data) {
        this.data = data;
    }

    public bool IsOutOfArea(Position pos) {
        // Check if the position is out of bounds
        if (pos.row < 0 || pos.col < 0 || pos.row >= data.GetLength(0) || pos.col >= data.GetLength(1)) {
            return true; // Out of bounds
        } else {
            return false;
        }
    }

    public bool IsBlocked(Position pos) {
        // Return whether the position is blocked
        return data[pos.row, pos.col];
    }

    public void Print(Guard guard, HashSet<Position> visited) {
        using (StreamWriter writer = new StreamWriter("debug.txt")) {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    if (guard.Position.Equals(new Position(i, j)))
                        writer.Write("O");
                    else if (visited.Contains(new Position(i, j)))
                        writer.Write("X");
                    else
                        writer.Write(data[i, j] ? '#' : '.');
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }
    }
}
class Direction {
    private static readonly IDictionary<char, Position> directions = new Dictionary<char, Position>() {
        { '^', new Position(-1, 0) },
        { '>', new Position(0, 1) },
        { 'v', new Position(1, 0) },
        { '<', new Position(0, -1) },
    };
    public Char Icon { get; init; }
    public Position Step { get; init; }

    public Direction(char c) {
        Icon = c;
        Step = directions[c];
    }
}

class Guard {
    private static readonly List<Direction> directions = new List<Direction>() {
        new Direction('^'),
        new Direction('>'),
        new Direction('v'),
        new Direction('<'),
    };

    public Position Position { get; set; }

    private int directionIdx;
    public Direction Direction {
        get => directions[directionIdx];
        set => directionIdx = directions.FindIndex(x => x.Equals(value));
    }

    public Guard(char c, Position pos) {
        Position = pos;
        directionIdx = directions.FindIndex(x => x.Icon == c);
    }

    public void Turn() {
        directionIdx = (directionIdx + 1) % directions.Count;
    }
}

public record Position(int row, int col) {
    public static Position operator +(Position p1, Position p2) => new Position(p1.row + p2.row, p1.col + p2.col);
}
