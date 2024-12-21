
using System.Collections.Immutable;

Console.WriteLine("Test");

var m = ParseFile();
m.buildGraphs();
// m.calculatePath();

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

    private List<Vector> walls;
    private Vector bounds;
    private Vector start;
    private Vector end;
    private Graph? graph;
    private readonly Dictionary<long, HashSet<Cheat>> savedCosts = new Dictionary<long, HashSet<Cheat>>();

    public Machine(List<Vector> walls, Vector bounds, Vector start, Vector end) {
        this.walls = walls;
        this.bounds = bounds;
        this.start = start;
        this.end = end;
    }

    public void buildGraphs() {
        savedCosts.Clear();
        graph = buildGraph(ImmutableList<Vector>.Empty);
        var (p, originalCost) = calculateCosts(end);
        System.Diagnostics.Debug.Assert(originalCost > -1);

        int step = 0;
        foreach (var wall in walls) {
            if (step++ % 500 == 0) {
                Console.WriteLine($"{step} of {walls.Count}");
            }

            removeWall(wall, originalCost);
        }
        // removeWall(new Vector(8, 1), originalCost);

        Console.WriteLine();
        foreach (var k in savedCosts.Keys.ToList().Order()) {
            Console.WriteLine($"{savedCosts[k].Count} {k}");
        }

        var sum = savedCosts.Where(kvp => kvp.Key >= 100).Select(kvp => kvp.Value.Count).Sum();
        Console.WriteLine(sum);
    }
    private void removeWall(Vector wall, long originalCost) {
        var toRemoveCombinations = new List<List<Vector>>() {
            new List<Vector>() { wall } // only remove current wall
        };

        // foreach (var step1 in ALL_STEPS) {
        //     var nextPos1 = wall + step1;
        //     if (!isWall(nextPos1)) {
        //         continue;
        //     }
        //     var toRemoveCombination = new List<Vector>() { wall, nextPos1 };

        //     bool isValidCheat = false;
        //     foreach (var step2 in ALL_STEPS) {
        //         var nextPos2 = nextPos1 + step2;
        //         if (!canByUsed(nextPos2, toRemoveCombination)) {
        //             isValidCheat = true;
        //             break;
        //         }
        //     }

        //     if (isValidCheat) {
        //         toRemoveCombinations.Add(toRemoveCombination);
        //     }
        // }

        foreach (var toRemoveCombination in toRemoveCombinations) {
            // Console.WriteLine(string.Join(" ", toRemoveCombination));

            // add new edges
            var addedEdges = new List<(Node from, Node to)>();
            foreach (var wallToRemove in toRemoveCombination) {
                addedEdges.AddRange(addEdges(graph, wallToRemove, toRemoveCombination));
                ALL_STEPS.ForEach(nextStep => addedEdges.AddRange(
                     addEdges(graph, wallToRemove + nextStep, toRemoveCombination)));
            }

            // calculate costs
            var (p, newCost) = calculateCosts(end);
            if (newCost > -1 && newCost < originalCost) {
                var wallIdx = p.FindIndex(node => toRemoveCombination.Contains(node.Pos));
                var start = p[wallIdx - 1].Pos;
                var endIdx = wallIdx;

                // walls not part of the path must be count also as coss
                var wallsPassed = 1;
                while (toRemoveCombination.Contains(p[++endIdx].Pos)) {
                    wallsPassed++;
                }
                var end = p[endIdx].Pos;
                var cheat = new Cheat(start, end);

                var diffCost = originalCost - newCost;

                savedCosts.TryAdd(diffCost, []);
                savedCosts[diffCost].Add(cheat);

                // Render(cheat, p, toRemoveCombination);
            }

            // reset graph
            addedEdges.ForEach(x => graph.RemoveEdge(x.from, x.to, 1));
        }
    }

     private void Render(Cheat c, List<Node> p, List<Vector> removed) {
        for (int y = 0; y < bounds.Y; y++) {
            for (int x = 0; x < bounds.X; x++) {
                var v = new Vector(x, y);
                if (c.start.Equals(v)) {
                    Console.Write("P");
                } else if (c.end.Equals(v)) {
                    Console.Write("S");
                } else if (removed.Contains(v)) {
                    Console.Write("X");
                } else if (walls.Contains(v)) {
                    Console.Write("#");
                } else if (p.Exists(node => node.Pos.Equals(v))) {
                    Console.Write("-");
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

    private (List<Node>? path, long cost) calculateCosts(Vector stop) {
        var (path, cost) = graph.FindShortestPath(new Node(start), new Node(stop));
        return (path, cost);
    }

    private Graph buildGraph(IReadOnlyCollection<Vector> wallSkipped) {
        var graph = new Graph();

        for (int x = 0; x < bounds.X; x++) {
            for (int y = 0; y < bounds.Y; y++) {
                addEdges(graph, new Vector(x, y), wallSkipped);
            }
        }

        return graph;
    }

    private List<(Node from, Node to)> addEdges(Graph graph, Vector from, IReadOnlyCollection<Vector> wallsIgnored) {
        var added = new List<(Node from, Node to)>();

        if (!canByUsed(from, wallsIgnored)) {
            return added;
        }

        var currentNode = new Node(from);
        foreach (var step in ALL_STEPS) {
            var nextNode = new Node(from + step);
            if (addEdges(graph, currentNode, nextNode, wallsIgnored)) {
                added.Add((currentNode, nextNode));
            }
        }

        return added;
    }

    private bool addEdges(Graph graph, Node currentNode, Node nextNode, IReadOnlyCollection<Vector> wallsIgnored) {
        if (canByUsed(nextNode.Pos, wallsIgnored)) {
            return graph.AddEdge(currentNode, nextNode, 1);
        } else if (wallsIgnored.Contains(nextNode.Pos)) {
            return graph.AddEdge(currentNode, nextNode, 1);
        }

        return false;
    }

    private bool canByUsed(Vector pos, IReadOnlyCollection<Vector> wallsIgnored) {
        if (!isInBound(pos)) {
            return false;
        } else if (isWall(pos)) {
            return wallsIgnored.Contains(pos);
        }

        return true;
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

    public (List<List<Node>>? paths, long cost) FindAllShortestPaths(Node source, Node destination) {
        if (!adjList.ContainsKey(source) || !adjList.ContainsKey(destination)) {
            Console.WriteLine("Source or destination does not exist in the graph.");
            return (null, -1);
        }

        var dist = new Dictionary<Node, long>();
        var paths = new Dictionary<Node, List<List<Node>>>();
        var pq = new SortedSet<(long, Node)>(Comparer<(long, Node)>.Create((a, b) => {
            int cmp = a.Item1.CompareTo(b.Item1); // Compare by cost
            return cmp != 0 ? cmp : a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode()); // Tie-breaking
        }));

        foreach (var node in adjList.Keys) {
            dist[node] = long.MaxValue;
            paths[node] = new List<List<Node>>();
        }

        dist[source] = 0;
        paths[source].Add(new List<Node> { source });
        pq.Add((0, source));

        while (pq.Count > 0) {
            var (currentCost, currentNode) = pq.Min;
            pq.Remove(pq.Min);

            foreach (var (neighbor, cost) in adjList[currentNode]) {
                long newCost = currentCost + cost;

                if (newCost < dist[neighbor]) {
                    pq.Remove((dist[neighbor], neighbor));

                    dist[neighbor] = newCost;
                    paths[neighbor].Clear();

                    foreach (var path in paths[currentNode]) {
                        var newPath = new List<Node>(path) { neighbor };
                        paths[neighbor].Add(newPath);
                    }

                    pq.Add((newCost, neighbor));
                } else if (newCost == dist[neighbor]) {
                    foreach (var path in paths[currentNode]) {
                        var newPath = new List<Node>(path) { neighbor };
                        paths[neighbor].Add(newPath);
                    }
                }
            }
        }

        if (dist[destination] == long.MaxValue) {
            return (null, -1); // No path found
        }

        return (paths[destination], dist[destination]);
    }

    public (List<Node>? path, long cost) FindShortestPath(Node source, Node destination) {
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

            if (currentNode.Equals(destination))
                break;

            foreach (var (neighbor, cost) in adjList[currentNode]) {
                long newCost = currentCost + cost;

                if (newCost < dist[neighbor]) {
                    pq.Remove((dist[neighbor], neighbor));
                    dist[neighbor] = newCost;
                    prev[neighbor] = currentNode;
                    pq.Add((newCost, neighbor));
                }
            }
        }

        if (dist[destination] == long.MaxValue) {
            return (null, -1);
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