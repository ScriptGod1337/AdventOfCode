
var endX = 70;
var endY = 70;
var walls = parseInput();

//part1();
part2();

void part1() {
    var m = new Machine(walls.GetRange(0, 1024), new Vector(endX + 1, endY + 1));
    m.Render();

    m.buildGraph();
    m.CalculatePath(new Vector(endX, endY));
}

void part2() {
    for (int n = 1025; n < walls.Count; n++) {
        Console.Write($"{n}... ");
        var m = new Machine(walls.GetRange(0, n), new Vector(endX + 1, endY + 1));
        m.buildGraph();
        if (!m.CalculatePath(new Vector(endX, endY))) {
            Console.WriteLine($"blocked by {walls[n - 1]}");
            break;
        }
         Console.WriteLine();
    }
}


List<Vector> parseInput() {
    var walls = new List<Vector>();
    foreach (var line in File.ReadLines("input.txt")) {
        var c = line.Split(",");
        walls.Add(new Vector(long.Parse(c[0]), long.Parse(c[1])));
    }

    return walls;
}

class Machine {
    private static readonly List<Vector> ALL_STEPS = new List<Vector>() {
        new Vector(0, 1),
        new Vector(0, -1),
        new Vector(1, 0),
        new Vector(-1, 0)
    };

    private List<Vector> walls;
    private Vector bounds;
    private Graph? graph;

    public Machine(List<Vector> walls, Vector bounds) {
        this.walls = walls;
        this.bounds = bounds;
    }

    public void buildGraph() {
        graph = new Graph();

        for (int x = 0; x < bounds.X; x++) {
            for (int y = 0; y < bounds.Y; y++) {
                addNodes(new Vector(x, y));
            }
        }
    }

    private void addNodes(Vector pos) {
        if (walls.Contains(pos)) {
            return;
        }

        var currentNode = new Node(pos);
        foreach (var step in ALL_STEPS) {
            addNodes(currentNode, pos + step, 1);
        }
    }

    private void addNodes(Node currentNode, Vector nextPos, long cost) {
        var nextNode = new Node(nextPos);
        if (walls.Contains(nextPos)) {
            return; // wall hit
        } else if ((nextNode.Pos.X < 0 || nextNode.Pos.Y < 0) || (nextNode.Pos.X > bounds.X || nextNode.Pos.Y > bounds.Y)) {
            return; // out of bounds
        }

        // Console.WriteLine($"Node {currentNode}:");
        // Console.WriteLine($"  -> {nextNode} with cost {cost}");
        graph.AddEdge(currentNode, nextNode, cost);
    }

    public bool CalculatePath(Vector vector) {
        var (p, cost) = graph.FindShortestPath(new Node(new Vector(0, 0)), new Node(vector));
        if (cost > 0) {
            Console.Write(p.Distinct().Count());
            return true;
        } else {
            return false;
        }
    }

    public void Render() {
        for (int y = 0; y < bounds.Y; y++) {
            for (int x = 0; x < bounds.X; x++) {
                var v = new Vector(x, y);
                if (walls.Contains(v)) {
                    Console.Write("#");
                } else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }
}

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

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }
}