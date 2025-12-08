namespace Common;

public class Matrix<TElement> {
    private readonly TElement[,] data;

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

    public void Print(StreamWriter writer) {
        Print(writer, null);
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