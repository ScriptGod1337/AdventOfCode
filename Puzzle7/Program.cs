part2("input.txt");

void part2(string file) {
    Matrix<long> matrix = Matrix<long>.LoadFile(
        file,
        c => c switch {
            'S' => -2,
            '^' => -1,
            _ => 0
        }
    );
    Vector2D? start = matrix.FindFirst(-2);
    if (start == null) {
        throw new Exception("Invalid input");
    }

    // set bean start
    matrix[start.X, start.Y] = 1;

    for (var row = start.X + 1; row < matrix.RowCount; row++) {
        for (var col = 0; col < matrix.ColumnCount; col++) {
            var above = matrix[row - 1, col];
            if (above <= 0) {
                continue; // no bean above -> skip
            }

            // increase number of hit bey beans
            var current = matrix[row, col];
            if (current < 0) {
                // split
                matrix[row, col - 1] += above;
                matrix[row, col + 1] += above;
            } else {
                matrix[row, col] += above;
            }
        }

        // using (StreamWriter writer = new StreamWriter(Console.OpenStandardOutput())) {
        //     matrix.Print(writer, x => x + "\t");
        // }
    }

    long sum = 0;
    foreach (var element in matrix.EnumerateInRow(matrix.RowCount - 1)) {
        sum += element;
    }

    Console.WriteLine($"sum {sum}");
}

void part1(string file) {
    Matrix<char> matrix = Matrix<char>.LoadFile(
        file,
        x => x
    );
    Vector2D? start = matrix.FindFirst('S');
    if (start == null) {
        throw new Exception("Invalid input");
    }

    // set bean start
    matrix[start.X, start.Y] = '|';

    var split = 0;
    for (var row = start.X + 1; row < matrix.RowCount; row++) {
        for (var col = 0; col < matrix.ColumnCount; col++) {
            var above = matrix[row - 1, col];
            if (above != '|') {
                continue; // no bean above -> skip
            }

            var current = matrix[row, col];
            if (current == '^') {
                // split
                matrix[row, col - 1] = '|';
                matrix[row, col + 1] = '|';
                split++;
            } else {
                matrix[row, col] = '|';
            }
        }

        // using (StreamWriter writer = new StreamWriter(Console.OpenStandardOutput())) {
        //     matrix.Print(writer);
        // }
    }

    Console.WriteLine($"split {split}");
}

class Matrix<TElement> {
    private TElement[,] data;

    public Matrix(TElement[,] data) {
        this.data = data;
    }

    public TElement this[long row, long column] {
        get => data[row, column];
        set => data[row, column] = value;
    }
    public long RowCount {
        get => data.GetLongLength(0);
    }

    public long ColumnCount {
        get => data.GetLongLength(1);
    }

    public Vector2D? FindFirst(TElement? element) {
        for (var row = 0; row < RowCount; row++) {
            for (var col = 0; col < ColumnCount; col++) {
                if (object.Equals(element, data[row, col])) {
                    return new (row, col);
                }
            }
        }

        return null;
    }

    public IEnumerable<TElement> EnumerateInRow(long row) {
        for (var col = 0; col < ColumnCount; col++)
            yield return data[row, col];
    }

    public IEnumerable<(long ColumnIndex, TElement Value)> EnumerateInWithIdx(long row) {
        for (var col = 0; col < ColumnCount; col++)
            yield return (col, data[row, col]);
    }

    public bool IsOutOfArea(Vector2D pos) {
        // Check if the position is out of bounds
        if (pos.X < 0 || pos.Y < 0 || pos.X >= data.GetLength(0) || pos.Y >= data.GetLength(1)) {
            return true; // Out of bounds
        } else {
            return false;
        }
    }

    public void Print(StreamWriter writer, Func<TElement, string>? conversion) {
        for (var row = 0; row < RowCount; row++) {
            for (var col = 0; col < ColumnCount; col++) {
                var element = data[row, col];
                writer.Write((conversion != null) ? conversion(element) : element);
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    public Matrix<TElementNew> Convert<TElementNew>(Func<TElement, TElementNew> conversion) {
        var newData = new TElementNew[RowCount, ColumnCount];

        for (var row = 0; row < RowCount; row++) {
            for (var col = 0; col < ColumnCount; col++) {
                newData[row, col] = conversion(data[row, col]);
            }
        }

        return new Matrix<TElementNew>(newData);
    }

    public static Matrix<TElement> LoadFile(string file, Func<char, TElement> conversion) {
        // Read all lines from the file
        var lines = File.ReadAllLines(file);

        // Determine the dimensions of the matrix
        var rows = lines.Length;
        var cols = lines[0].Length;

        // Create the matrix
        var data = new TElement[rows, cols];

        // Fill the matrix
        for (var row = 0; row < rows; row++) {
            for (var col = 0; col < cols; col++) {
                data[row, col] = conversion(lines[row][col]);
            }
        }

        return new Matrix<TElement>(data);
    }
}

record Vector2D(long X, long Y) {
    public static Vector2D operator +(Vector2D p1, Vector2D p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Vector2D operator -(Vector2D p1, Vector2D p2) => new(p1.X - p2.X, p1.Y - p2.Y);
    public static Vector2D operator %(Vector2D p1, Vector2D p2) => new(p1.X % p2.X, p1.Y % p2.Y);
    public static Vector2D operator *(Vector2D p, long factor) => new(p.X * factor, p.Y * factor);
    public static Vector2D operator *(long factor, Vector2D p) => new(p.X * factor, p.Y * factor);
    public static Vector2D operator /(Vector2D p, long factor) => new(p.X / factor, p.Y / factor);
    public static Vector2D operator /(long factor, Vector2D p) => new(p.X / factor, p.Y / factor);
}