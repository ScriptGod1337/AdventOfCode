part2();

#region part2
void part2() {
    (var matrix, var indices) = parseFile();

    var x = indices
        .Select(kvp => (kvp.Value, buildGroup(kvp.Key)))
        .Select(kvp => calculate(kvp.Item1, kvp.Item2))
        .Sum();
    Console.WriteLine($"sum {x}");
    
    List<long> buildGroup(int index) {
        List<long> result = new();

        // define and and start column of the group
        var startCol = index;
        var endCol = indices.Keys
            .Order()
            .FirstOrDefault(nextIndex => (nextIndex > index), matrix.GetLength(1));

        // build a string of each row
        // ...and convert to a number
        var rows = matrix.GetLength(0);
        for (var n = index; n < endCol; n++) {
            var str = new string(Enumerable.Range(0, rows)
                .Select(r => matrix[r, n])
                .ToArray()).Trim();
            long num = 0;
            if (long.TryParse(str, out num)) {
                result.Add(num);
            }
        }

        return result;
    }

    long calculate(char op, List<long> group) {
        long result;
        switch (op) {
            case '+':
                result = group.Sum();
                break;
            case '*':
                result = group.Aggregate((a, b) => a * b);
                break;
            default:
                throw new Exception("Invalid operation");
        }

        return result;
    }

    (char[,], Dictionary<int, char>) parseFile() {
        var lines = File.ReadAllLines("input.txt");

        var indices = lines.Last()
            .Select((ch, i) => (ch, i))
            .Where(t => t.ch != ' ')
            .ToDictionary(t => t.i, t => t.ch);
            
        return (parseFileNumbers(lines.SkipLast(1).ToArray()), indices);
    }

    char[,] parseFileNumbers(string[] numberLines) {
        int rows = numberLines.Length;
        int cols = numberLines[0].Length;

        char[,] matrix = new char[rows, cols];

        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                matrix[r, c] = numberLines[r][c];
            }
        }

        return matrix;
    }
}
#endregion

#region part1
void part1() {

    var rows = File.ReadAllLines("input.txt")
        .Select(line => line
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList())
        .ToList();
    var columns = Enumerable.Range(0, rows[0].Count)
        .Select(col => rows.Select(row => row[col]).ToList())
        .ToList();

    var sum = columns.Select(calculate).Sum();
    Console.WriteLine($"sum {sum}");

    long calculate(List<string> colunn) {
        var colNums = colunn
            .SkipLast(1)
            .Select(long.Parse)
            .ToList();

        long result;
        switch (colunn.Last()) {
            case "+":
                result = colNums.Sum();
                break;
            case "*":
                result = colNums.Aggregate((a, b) => a * b);
                break;
            default:
                throw new Exception("Invalid operation");
        }

        return result;
    }
}

#endregion