using Common;

part1("input.txt");

#region part1
void part1(string file) {
    double totalCost = 0;
    foreach (var line in File.ReadAllLines(file)) {
        var (targetState, buttons) = ParseLine(line);

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

        Console.WriteLine($"graphCosts: {currentCosts}");
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
#endregion

(MachineState, IEnumerable<MachineState>) ParseLine(string line) {
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
    var joylts = parts.Last()
        .Trim('{', '}')
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToList();

    return (targetState, buttons);
}

record MachineState(int State) {
    public override string ToString()
        => new ([.. Convert.ToString(State, 2).Reverse()]);
}

