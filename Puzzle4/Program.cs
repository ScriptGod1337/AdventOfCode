var world = ParseFile("input.txt");

part2(world);

#region part2
void part2(World world) {
    var data = world.Data;
    int rows = data.GetLength(0);
    int cols = data.GetLength(1);
    
    var valid = 0;
    var removed = false;
    do {
        removed = false;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                var pos = new Vector(i, j);
                if (!world.IsMarked(pos)) {
                    continue; // only checked marked once
                } else if (checkSuroundings(world, pos)) {
                    valid++;
                    data[i, j] = false;
                    removed = true;
                }
            }
        }
    } while(removed);

    Console.WriteLine($"valid {valid}");
}
#endregion

#region part1
void part1(World world) {
    var data = world.Data;
    int rows = data.GetLength(0);
    int cols = data.GetLength(1);
    
    var valid = 0;
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            var pos = new Vector(i, j);
            if (!world.IsMarked(pos)) {
                continue; // only checked marked once
            } else if (checkSuroundings(world, pos)) {
                valid++;
            }
        }
    }

    Console.WriteLine($"valid {valid}");
}
#endregion

bool checkSuroundings(World world, Vector pos) {
    var marked = 0;
    for (var x = -1; x <= 1; x++) {
        for (var y = -1; y <= 1; y++) {
            var v = pos + new Vector(x, y);
            if (((x == 0) && (y == 0)) || world.IsOutOfArea(v)) {
                continue;
            }

            if (world.IsMarked(v)) {
                marked++;
            }
        }
    }

    return (marked < 4);
}

World ParseFile(string filePath) {
    // Read all lines from the file
    string[] lines = File.ReadAllLines(filePath);

    // Determine the dimensions of the matrix
    int rows = lines.Length;
    int cols = lines[0].Length;

    // Create the matrix
    bool[,] matrix = new bool[rows, cols];

    // Fill the matrix
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            matrix[i, j] = (lines[i][j] == '@');
        }
    }

    return new World(matrix);
}

class World {
    public bool[,] Data;

    public World(bool[,] data) {
        this.Data = data;
    }

    public bool IsOutOfArea(Vector pos) {
        // Check if the position is out of bounds
        if (pos.X < 0 || pos.Y < 0 || pos.X >= Data.GetLength(0) || pos.Y >= Data.GetLength(1)) {
            return true; // Out of bounds
        } else {
            return false;
        }
    }

    public bool IsMarked(Vector pos) {
        // Return whether the position is blocked
        return Data[pos.X, pos.Y];
    }
}

record Vector(long X, long Y) {
    public static Vector operator +(Vector p1, Vector p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Vector operator -(Vector p1, Vector p2) => new(p1.X - p2.X, p1.Y - p2.Y);
    public static Vector operator %(Vector p1, Vector p2) => new(p1.X % p2.X, p1.Y % p2.Y);
    public static Vector operator *(Vector p, long factor) => new(p.X * factor, p.Y * factor);
    public static Vector operator *(long factor, Vector p) => new(p.X * factor, p.Y * factor);
    public static Vector operator /(Vector p, long factor) => new(p.X / factor, p.Y / factor);
    public static Vector operator /(long factor, Vector p) => new(p.X / factor, p.Y / factor);
}