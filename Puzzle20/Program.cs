
var m = ParseFile();
m.cheat();

Machine ParseFile() {
    var walls = new List<Vector>();
    Vector start = null;
    Vector end = null;

    string[] lines = File.ReadAllLines("input.txt");
    for (int y = 0; y < lines.Length; y++) {
        string line = lines[y];
        for (int x = 0; x < line.Length; x++) {
            switch (line[x]) {
                case '#':
                    walls.Add(new Vector(x, y));
                    break;
                case 'S':
                    start = new Vector(x, y);
                    break;
                case 'E':
                    end = new Vector(x, y);
                    break;
            }

        }
    }
    if (start == null || end == null) {
        throw new ArgumentException();
    }
    Vector bounds = new Vector(lines[0].Length, lines.Length);

    var machine = new Machine(walls, bounds, start, end);
    return machine;
}

class Machine {
    private record Cheat(Vector start, Vector end);

    private static readonly List<Vector> ALL_STEPS = new List<Vector>() {
        new Vector(0, 1),
        new Vector(0, -1),
        new Vector(1, 0),
        new Vector(-1, 0)
    };

    private HashSet<Vector> walls;
    private Vector bounds;
    private Vector raceStart;
    private Vector raceEnd;
    private Graph? graph;

    public Machine(List<Vector> walls, Vector bounds, Vector start, Vector end) {
        this.walls = new HashSet<Vector>(walls);
        this.bounds = bounds;
        this.raceStart = start;
        this.raceEnd = end;
    }

    private List<(Cheat cheat, long cost)> findPossibleCheats(Vector cheatStart, int maxStep) {
        var allCheats = new List<(Cheat, long)>();

        if (cheatStart.Equals(raceStart))
            Console.WriteLine();

        for (var x = -maxStep; x <= maxStep; x++) {
            for (var y = -maxStep; y <= maxStep; y++) {
                var cheatEnd = cheatStart + new Vector(x, y);
                var cost = Math.Abs(x) + Math.Abs(y);
                if ((cost > maxStep)
                    || (cost <= 1)
                    || !isInBound(cheatEnd)
                    || isWall(cheatEnd)
                ) {
                    continue;
                }

                allCheats.Add((new Cheat(cheatStart, cheatEnd), cost));
            }
        }

        return allCheats;
    }

    public void cheat() {
        buildGraph();

        // get potential cheats
        var allPossibleCheats = calcPossibleCheats(20);

        // get path and build lookup for it
        var originalPath = calculateOriginal().Select(element => element.Pos).ToList();
        var originalPathLookup = originalPath //
            .Select((element, index) => new { element, index })
            .ToDictionary(e => e.element, e => e.index);

        var costs = new Dictionary<long, long>();
        for (var n = 0; n < originalPath.Count; n++) {
            if (n % 100 == 0) {
                Console.WriteLine($"{n + 1} of {originalPath.Count}");
            }

            Dictionary<Vector, long>? possibleCheats;
            if (!allPossibleCheats.TryGetValue(originalPath[n], out possibleCheats) || possibleCheats == null) {
                continue;
            }

            foreach (var (endPos, cheatCost) in possibleCheats) {
                int cheatEndPosIdx;
                if (!originalPathLookup.TryGetValue(endPos, out cheatEndPosIdx)
                    || cheatEndPosIdx <= n) {
                    continue;
                }

                var diff = cheatEndPosIdx - n - cheatCost;
                costs.TryAdd(diff, 0);
                costs[diff]++;
            }
        }

        Console.WriteLine();
        foreach (var k in costs.Keys.ToList()
            // .Where(x => (x > 49))
            .Order()) {
            Console.WriteLine($"{costs[k]} {k}");
        }

        var sum = costs.Where(kvp => (kvp.Key >= 100)).Select(kvp => kvp.Value).Sum();
        Console.WriteLine(sum);
    }

    private Dictionary<Vector, Dictionary<Vector, long>> calcPossibleCheats(int depth) {
        var allPossibleCheats = new Dictionary<Vector, Dictionary<Vector, long>>();
        for (int x = 0; x < bounds.X; x++) {
            for (int y = 0; y < bounds.Y; y++) {
                var cheatStart = new Vector(x, y);
                if (!isWall(cheatStart)) {
                    findPossibleCheats(cheatStart, depth).ToList().ForEach(x => {
                        allPossibleCheats.TryAdd(x.cheat.start, []);
                        allPossibleCheats[x.cheat.start].Add(x.cheat.end, x.cost);
                    });
                }
            }
        }

        return allPossibleCheats;
    }

    private List<Node> calculateOriginal() {
        var results = graph.FindShortestPath(new Node(raceStart), new Node(raceEnd));
        System.Diagnostics.Debug.Assert(results.cost > 0);
        return results.path;
    }

    private void Render(List<Node> p, Vector pos) {
        for (int y = 0; y < bounds.Y; y++) {
            for (int x = 0; x < bounds.X; x++) {
                var v = new Vector(x, y);
                if (pos.Equals(v)) {
                    Console.Write("S");
                } else if (p.Exists(node => node.Pos.Equals(v))) {
                    if (isWall(v)) {
                        Console.Write("X");
                    } else {
                        Console.Write("-");
                    }
                } else if (walls.Contains(v)) {
                    Console.Write("#");
                } else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }

    public void Render(IList<Vector> pre, IList<Vector> suc) {
        for (int y = 0; y < bounds.Y; y++) {
            for (int x = 0; x < bounds.X; x++) {
                var v = new Vector(x, y);
                if (pre.Contains(v)) {
                    Console.Write("P");
                } else if (suc.Contains(v)) {
                    Console.Write("S");
                } else if (walls.Contains(v)) {
                    Console.Write("#");
                } else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }

    private void buildGraph() {
        graph = new Graph();

        for (int x = 0; x < bounds.X; x++) {
            for (int y = 0; y < bounds.Y; y++) {
                addEdges(graph, new Vector(x, y));
            }
        }
        // graph.PrintGraph();
    }

    private void addEdges(Graph graph, Vector from) {
        if (!isInBound(from)) {
            return;
        }

        var currentNode = new Node(from);
        ALL_STEPS.ForEach(step => addEdges(graph, from, from + step));
    }

    private void addEdges(Graph graph, Vector from, Vector to) {
        if (!isInBound(to) || isWall(to)) {
            return;
        }

        graph.AddEdge(new Node(from), new Node(to), 1);
    }

    private bool isWall(Vector pos) {
        return walls.Contains(pos);
    }

    private bool isInBound(Vector pos) {
        if ((pos.X < 0 || pos.Y < 0) || (pos.X > bounds.X || pos.Y > bounds.Y)) {
            return false;
        } else {
            return true;
        }
    }
}

#region graph
record Node(Vector Pos);

class Graph {
    private readonly Dictionary<Node, List<(Node neighbor, long cost)>> adjList;

    public Graph() {
        adjList = new Dictionary<Node, List<(Node, long)>>();
    }

    public void AddNode(Node node) {
        if (!adjList.ContainsKey(node)) {
            adjList[node] = new List<(Node, long)>();
        }
    }

    public bool AddEdge(Node from, Node to, long cost) {
        AddNode(from);

        // Check if the edge already exists
        if (adjList[from].Exists(edge => edge.neighbor == to && edge.cost == cost)) {
            return false; // Edge already exists
        }

        AddNode(to);
        adjList[from].Add((to, cost));
        return true; // New edge added
    }

    public bool RemoveEdge(Node from, Node to, long cost) {
        if (!adjList.ContainsKey(from)) return false;

        // Find and remove the edge if it exists
        var removed = adjList[from].RemoveAll(edge => edge.neighbor.Equals(to) && edge.cost == cost);

        return removed > 0; // Return true if at least one edge was removed
    }

    public (List<Node>? path, long cost) FindShortestPath(Node source, Node destination, long? maxCosts = null) {
        if (!adjList.ContainsKey(source) || !adjList.ContainsKey(destination)) {
            Console.WriteLine("Source or destination does not exist in the graph.");
            return (null, -1);
        }

        var dist = new Dictionary<Node, long>();
        var prev = new Dictionary<Node, Node>();
        var pq = new SortedSet<(long, Node)>(Comparer<(long, Node)>.Create((a, b) => {
            int cmp = a.Item1.CompareTo(b.Item1);
            return cmp != 0 ? cmp : a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode());
        }));

        foreach (var node in adjList.Keys) {
            dist[node] = long.MaxValue;
            prev[node] = null;
        }

        dist[source] = 0;
        pq.Add((0, source));

        while (pq.Count > 0) {
            var (currentCost, currentNode) = pq.Min;
            pq.Remove(pq.Min);

            // If the current cost exceeds maxCosts, prune this path
            if (maxCosts.HasValue && currentCost > maxCosts.Value) {
                continue;
            }

            if (currentNode.Equals(destination)) {
                break;
            }

            foreach (var (neighbor, cost) in adjList[currentNode]) {
                long newCost = currentCost + cost;

                // Skip if new cost exceeds maxCosts
                if (maxCosts.HasValue && newCost > maxCosts.Value) {
                    continue;
                }

                if (newCost < dist[neighbor]) {
                    pq.Remove((dist[neighbor], neighbor));
                    dist[neighbor] = newCost;
                    prev[neighbor] = currentNode;
                    pq.Add((newCost, neighbor));
                }
            }
        }

        if (dist[destination] == long.MaxValue || (maxCosts.HasValue && dist[destination] > maxCosts.Value)) {
            return (null, -1); // No path found within maxCosts
        }

        var path = new List<Node>();
        for (var at = destination; at != null; at = prev[at]) {
            path.Add(at);
        }

        path.Reverse();
        return (path, dist[destination]);
    }


    public void PrintGraph() {
        foreach (var node in adjList) {
            Console.WriteLine($"Node {node.Key}:");
            foreach (var edge in node.Value) {
                Console.WriteLine($"  -> {edge.neighbor} with cost {edge.cost}");
            }
        }
    }
}

record Vector(long X, long Y) {
    public static Vector operator +(Vector p1, Vector p2) => new Vector(p1.X + p2.X, p1.Y + p2.Y);
    public static Vector operator -(Vector p1, Vector p2) => new Vector(p1.X - p2.X, p1.Y - p2.Y);
    public static Vector operator %(Vector p1, Vector p2) => new Vector(p1.X % p2.X, p1.Y % p2.Y);
    public static Vector operator *(Vector p, long factor) => new Vector(p.X * factor, p.Y * factor);
    public static Vector operator *(long factor, Vector p) => new Vector(p.X * factor, p.Y * factor);
    public static Vector operator /(Vector p, long factor) => new Vector(p.X / factor, p.Y / factor);
    public static Vector operator /(long factor, Vector p) => new Vector(p.X / factor, p.Y / factor);
}
#endregion