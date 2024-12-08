// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var (matrix, lookup) = parseFile("input.txt");
part();

void part() {
    var anti = new HashSet<Position>();
    foreach (var (k, v) in lookup) {
        if (!char.IsLetterOrDigit(k)) {
            continue;
        }

        anti.UnionWith(findAntiNodes2(v, matrix));
    }

    Console.WriteLine(anti.Count);
}

HashSet<Position> findAntiNodes2(List<Position> positions, char[,] data) {
    var anti = new HashSet<Position>();
    for (var i = 0; i < positions.Count; i++) {
        for (var j = 0; j < positions.Count; j++) {
            if (i == j) {
                continue;
            }

            var pos1 = positions[i];
            var pos2 = positions[j];
            anti.Add(pos1);
            anti.Add(pos2);

            var distance = pos2.Distance(pos1);
            var next = pos1 - distance;
            while (!IsOutOfArea(next, data)) {
                anti.Add(next);
                next -= distance;
            }
        }
    }

    return anti;
}

HashSet<Position> findAntiNodes1(List<Position> positions, char[,] data) {
    var anti = new HashSet<Position>();
    for (var i = 0; i < positions.Count; i++) {
        for (var j = 0; j < positions.Count; j++) {
            if (i == j) {
                continue;
            }

            var pos1 = positions[i];
            var pos2 = positions[j];
            var distance = pos2.Distance(pos1);

            var next = pos1 - distance;
            if (!IsOutOfArea(next, data)) {
                anti.Add(next);
            }
        }
    }

    return anti;
}

bool IsOutOfArea(Position pos, char[,] data) {
    // Check if the position is out of bounds
    if (pos.row < 0 || pos.col < 0 || pos.row >= data.GetLength(0) || pos.col >= data.GetLength(1)) {
        return true; // Out of bounds
    } else {
        return false;
    }
}

(char[,], Dictionary<char, List<Position>>) parseFile(string filePath) {
    // Read all lines from the file
    string[] lines = File.ReadAllLines(filePath);

    // Determine the dimensions of the matrix
    int rows = lines.Length;
    int cols = lines[0].Length;

    // Create the matrix
    char[,] matrix = new char[rows, cols];

    // Fill the matrix
    var lookup = new Dictionary<char, List<Position>>();
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            matrix[i, j] = lines[i][j];
            lookup.TryAdd(lines[i][j], new List<Position>());
            lookup[lines[i][j]].Add(new Position(i, j));
        }
    }

    return (matrix, lookup);
}

public record Position(int row, int col) {
    public Position Distance(Position p2) => new Position(row - p2.row, col - p2.col);
    public static Position operator +(Position p1, Position p2) => new Position(p1.row + p2.row, p1.col + p2.col);
    public static Position operator -(Position p1, Position p2) => new Position(p1.row - p2.row, p1.col - p2.col);
}
