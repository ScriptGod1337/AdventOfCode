using Common;
using Microsoft.Z3;

// part1("test.txt");
// part1("input.txt");
//part2("test.txt");
part2("input.txt");

#region part2
void part2(string file) {
    long totalCost = 0;
    foreach (var line in File.ReadAllLines(file)) {
        var (buttons, joltages) = ParseLinePart2(line);

        // create variable matrix
        // 1. for each joltage, create a row
        // 2. for each button, set column of that row to 1 if button affects that joltage
        var varMatrix = new long[joltages.Count, buttons.Count];
        for (var n = 0; n < joltages.Count; n++) {
            for (var m = 0; m < buttons.Count; m++) {
                varMatrix[n, m] = buttons[m].Contains(n) ? 1 : 0;
            }
        }

        // create constant matrix
        var constants = new long[joltages.Count];
        for (var n = 0; n < joltages.Count; n++) {
            constants[n] = joltages[n];
        }

        var solution = SolveNonNegative(varMatrix, constants);
        var currentCosts = solution.Sum();
        totalCost += currentCosts;
        
        Console.WriteLine($"currentCosts: {currentCosts} - buttons# {buttons.Count}");
    }

    Console.WriteLine($"totalCost: {totalCost}");
}

long[] SolveNonNegative(long[,] A, long[] b, long bound = 1000) {
    int m = A.GetLength(0);
    int n = A.GetLength(1);

    using var ctx = new Context();
    var opt = ctx.MkOptimize();

    IntExpr[] x = new IntExpr[n];
    for (int i = 0; i < n; i++) {
        x[i] = ctx.MkIntConst($"x{i}");

        // x >= 0 (and optional upper bound)
        opt.Add(ctx.MkGe(x[i], ctx.MkInt(0)));
        opt.Add(ctx.MkLe(x[i], ctx.MkInt(bound)));
    }

    // Ax = b
    for (int r = 0; r < m; r++) {
        ArithExpr sum = ctx.MkInt(0);
        for (int c = 0; c < n; c++) {
            sum = ctx.MkAdd(sum, ctx.MkMul(ctx.MkInt(A[r, c]), x[c]));
        }

        opt.Add(ctx.MkEq(sum, ctx.MkInt(b[r])));
    }

    // Minimize sum(x)
    ArithExpr obj = ctx.MkInt(0);
    for (int i = 0; i < n; i++) {
        obj = ctx.MkAdd(obj, x[i]);
    }

    opt.MkMinimize(obj);

    if (opt.Check() != Status.SATISFIABLE) {
        throw new Exception("No feasible non-negative integer solution under given bounds.");
    }

    var model = opt.Model;
    var sol = new long[n];
    for (int i = 0; i < n; i++) {
        sol[i] = ((IntNum)model.Evaluate(x[i], true)).Int64;
    }

    return sol;
}

(List<List<int>> Buttons, List<int> Joltages) ParseLinePart2(string line) {
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // 1) pattern string
    // ...skip

    // 2) list of number-lists from parentheses
    var buttons = parts
        .Skip(1)
        .TakeWhile(p => p.StartsWith("("))
        .Select(p => p.Trim('(', ')')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList()
        )
        .ToList();

    // 3) final number list from braces
    var joltages = parts.Last()
        .Trim('{', '}')
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToList();

    return (buttons, joltages);
}
#endregion

#region part1
void part1(string file) {
    double totalCost = 0;
    foreach (var line in File.ReadAllLines(file)) {
        var (targetState, buttons) = ParseLinePart1(line);

        // build graph
        var tree = new Tree<MachineState, MachineState>();
        var initState = new MachineState(0);
        tree.AddNode(initState);
        BuildTree(tree, initState, buttons);

        tree.TryShortestPath(
            initState,
            targetState,
            step => 1.0,
            out var path,
            out var currentCosts
        );
        totalCost += currentCosts;

        Console.WriteLine($"currentCosts: {currentCosts}");
    }

    Console.WriteLine($"totalCost: {totalCost}");
}
void BuildTree(Tree<MachineState, MachineState> tree, MachineState parent, IEnumerable<MachineState> buttons) {
    foreach (var button in buttons) {
        var nextState = new MachineState(parent.State ^ button.State);

        if (tree.AddEdge(parent, nextState, button)) {
            BuildTree(tree, nextState, buttons);
        }
    }
}

(MachineState TargetState, IEnumerable<MachineState> Buttons) ParseLinePart1(string line) {
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // 1) pattern string
    var targetState = new MachineState(parts[0] // "[.##.]"
            .Trim('[', ']')
            .Select((c, index) => (c == '#') ? 1 << index : 0)
            .Aggregate((a, b) => a | b)
    );

    // 2) list of number-lists from parentheses
    var buttons = parts
        .Skip(1)
        .TakeWhile(p => p.StartsWith("("))
        .Select(p => p.Trim('(', ')')
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .Select(n => 1 << n)
            .Aggregate((a, b) => a | b)
        )
        .Select(n => new MachineState(n))
        .ToList();

    // 3) final number list from braces
    // ...skip

    return (targetState, buttons);
}
record MachineState(int State) {
    public override string ToString()
        => new ([.. Convert.ToString(State, 2).Reverse()]);
}

#endregion