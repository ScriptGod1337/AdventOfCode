part1();

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