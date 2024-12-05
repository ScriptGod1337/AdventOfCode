// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!4");

var matrix = readInputMatrix();

var count = 0;
#if false
var pattern = "XMAS";
part1();
#endif

#if true
var pattern = "MAS";
part2();
#endif

#region part1
void part1() {
    for (int currentRow = 0; currentRow < matrix.Count; currentRow++) {
        var row = matrix[currentRow];
        for (int currentCol = 0; currentCol < row.Count; currentCol++) {
            searchPattern(matrix, currentRow, currentCol, 0, 1);
            searchPattern(matrix, currentRow, currentCol, 1, -1);
            searchPattern(matrix, currentRow, currentCol, 1, 0);
            searchPattern(matrix, currentRow, currentCol, 1, 1);
        }
    }
}

void searchPattern(List<List<char>> matrix, int currentRow, int currentCol, int rowIncrement, int colIncrement) {
    var tree = makeTree(matrix, currentRow, currentCol, rowIncrement, colIncrement);
    if (testPattern(tree)) {
        count++;
    }
}

List<char> makeTree(List<List<char>> matrix, int currentRow, int currentCol, int rowIncrement, int colIncrement, List<char> elements = null) {
    if (elements == null) {
        elements = new List<char>();
    }
    if (elements.Count == pattern.Length) {
        return elements;
    }

    if (!isInBound(matrix, currentRow, currentCol)) {
        return elements;
    }

    elements.Add(matrix[currentRow][currentCol]);
    makeTree(matrix, currentRow + rowIncrement, currentCol + colIncrement, rowIncrement, colIncrement, elements);
    return elements;
}

#endregion

#region part2
void part2() {
    for (int currentRow = 0; currentRow < matrix.Count; currentRow++) {
        var row = matrix[currentRow];
        for (int currentCol = 0; currentCol < row.Count; currentCol++) {
            if (isXMas(matrix, currentRow, currentCol)) {
                count++;
            }
        }
    }
}

bool isXMas(List<List<char>> matrix, int rowIdx, int colIdx) {
    var start = getChar(matrix, rowIdx, colIdx).Value;
    if (!start.Equals('M') && !start.Equals('S')) {
        return false;
    }

    var left = ToList(
        getChar(matrix, rowIdx, colIdx),
        getChar(matrix, rowIdx + 1, colIdx + 1),
        getChar(matrix, rowIdx + 2, colIdx + 2)
    ).Where(x => x.HasValue).Select(x => x.Value);
    var right = ToList(
        getChar(matrix, rowIdx, colIdx + 2),
        getChar(matrix, rowIdx + 1, colIdx + 1),
        getChar(matrix, rowIdx + 2, colIdx)
    ).Where(x => x.HasValue).Select(x => x.Value);

    return testPattern(left) && testPattern(right);
}

char? getChar(List<List<char>> matrix, int rowIdx, int colIdx) {
    if (!isInBound(matrix, rowIdx, colIdx)) {
        return null;
    }

    return matrix[rowIdx][colIdx];
}

List<T> ToList<T>(params T[] items) {
    return new List<T>(items);
}
#endregion

Console.WriteLine("Hello, World!4");

List<List<char>> readInputMatrix() {
    List<List<char>> matrix = new List<List<char>>();

    foreach (string line in File.ReadAllLines("input.txt")) {
        List<char> row = new List<char>(line);
        matrix.Add(row);
    }

    return matrix;
}

bool isInBound(List<List<char>> matrix, int rowIdx, int colIdx) {
    if (matrix.Count <= rowIdx) {
        return false;
    }
    var row = matrix[rowIdx];
    if (colIdx < 0 || row.Count <= colIdx) {
        return false;
    }

    return true;
}

bool testPattern(IEnumerable<char> chars) {
    var list = chars.ToList();
    if (list.Count < pattern.Length) {
        return false;
    }

    var joined = string.Join("", list);
    var joinedReverse = new string(joined.Reverse().ToArray());
    return (pattern.Equals(joined) || pattern.Equals(joinedReverse));
}
