// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var matrix = ParseFileToMatrix("input.txt");
var steps = new List<Position>() {
    new Position(0, -1), // left
    new Position(0, 1), // right
    new Position(1, 0), // down
    new Position(-1, 0), // up
};
processMatrix2();

#region part2
void processMatrix2() {
    int rows = matrix.GetLength(0);
    int cols = matrix.GetLength(1);

    var sum = 0;
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (matrix[i, j] == 0) {
                var posResult = new List<List<Position>>();
                traverse2(new Position(i, j), null, matrix, null, posResult);
                sum += posResult.Count;
            }
        }
    }

    Console.WriteLine(sum);
}

void traverse2(Position current, int? lastNum, int[,] matrix, List<Position> path, ICollection<List<Position>> results) {
    // Console.WriteLine($"Processing: {current} {lastNum}");
    if (path == null) {
        path = new List<Position>();
    }
    path.Add(current);

    var currentNum = matrix[current.row, current.col];
    if ((lastNum != null) && (lastNum + 1) != currentNum) {
        return; // incorrect step
    }
    if (currentNum == 9) {
        results.Add(path);
        return; // end
    }

    foreach (var step in steps) {
        var next = current + step;
        if (IsOutOfArea(next, matrix)) {
            continue;
        }

        traverse2(next, currentNum, matrix, new List<Position>(path), results);
    }
}
#endregion

#region part1
void processMatrix1() {
    var results = new Dictionary<Position, ICollection<Position>>();

    int rows = matrix.GetLength(0);
    int cols = matrix.GetLength(1);

    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (matrix[i, j] == 0) {
                var posResult = new HashSet<Position>();
                results.Add(new Position(i, j), posResult);
                traverse1(new Position(i, j), matrix, null, posResult);
            }
        }
    }

    var sum = results.Values.Sum(collection => collection.Count);
    Console.WriteLine(sum);
}

void traverse1(Position current, int[,] matrix, int? lastNum, ICollection<Position> results) {
    // Console.WriteLine($"Processing: {current} {lastNum}");

    var currentNum = matrix[current.row, current.col];
    if ((lastNum != null) && (lastNum + 1) != currentNum) {
        return; // incorrect step
    }
    if (currentNum == 9) {
        results.Add(current);
        return; // end
    }

    foreach (var step in steps) {
        var next = current + step;
        if (IsOutOfArea(next, matrix)) {
            continue;
        }

        traverse1(next, matrix, currentNum, results);
    }
}
#endregion

bool IsOutOfArea(Position pos, int[,] matrix) {
    // Check if the position is out of bounds
    if (pos.row < 0 || pos.col < 0 || pos.row >= matrix.GetLength(0) || pos.col >= matrix.GetLength(1)) {
        return true; // Out of bounds
    } else {
        return false;
    }
}

int[,] ParseFileToMatrix(string filePath) {
    var lines = File.ReadAllLines(filePath);
    int rowCount = lines.Length;
    int colCount = lines[0].Length; // Assuming all rows have the same length

    var matrix = new int[rowCount, colCount];

    for (int i = 0; i < rowCount; i++) {
        for (int j = 0; j < colCount; j++) {
            matrix[i, j] = lines[i][j] - '0'; // Convert char digit to int
        }
    }

    return matrix;
}

record Position(int row, int col) {
    public static Position operator +(Position p1, Position p2) => new Position(p1.row + p2.row, p1.col + p2.col);
    public static Position operator -(Position p1, Position p2) => new Position(p1.row - p2.row, p1.col - p2.col);
}