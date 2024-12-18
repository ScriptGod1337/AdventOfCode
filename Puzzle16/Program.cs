Console.WriteLine("Reading input...");
var machine = ParseFile();
Console.WriteLine("Calculate path...");
machine.CalculatePath();

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

    var machine = new Machine(walls, start, end);
    machine.buildGraph(lines[0].Length, lines.Length);
    return machine;
}

record Node(Vector Pos, Vector Dir);

class Machine {
    private static readonly List<Vector> ALL_STEPS = new List<Vector>() {
        new Vector(0, 1),
        new Vector(0, -1),
        new Vector(1, 0),
        new Vector(-1, 0)
    };

    private List<Vector> walls;
    private Vector start;
    private Vector end;

    private Graph? graph;

    public Machine(List<Vector> walls, Vector start, Vector end) {
        this.walls = walls;
        this.start = start;
        this.end = end;
    }

    public void buildGraph(int sizeX, int sizeY) {
        graph = new Graph();

        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                addNodes(new Vector(x, y));
            }
        }
    }

    public void CalculatePath() {
        var look = new Vector(1, 0); /* east */
        var nodeStart = new Node(start, look);

        var firstShortest = ALL_STEPS.Select(step => graph.FindShortestPath(nodeStart, new Node(end, step))) //
            .Where(x => x.cost > 0) //
            .GroupBy(x => x.cost) //
            .ToDictionary(group => group.Key, //
                group => group.Select(x => x.path).ToList());
        Console.WriteLine($"Min cost {firstShortest.Keys.Min()}");
        
        // asssuming(!) the end node can only be reached by one direction
        // ...to speed up
        Node endNode = null;
        foreach (var p in firstShortest[firstShortest.Keys.Min()]) {
            var node = p.Last();
            if (endNode != null) {
                System.Diagnostics.Debug.Assert(node.Equals(endNode));
            }
            endNode = node;
        }
        System.Diagnostics.Debug.Assert(endNode != null);

        var visitedPos = new HashSet<Vector>();
        var allPath = graph.FindAllShortestPaths(nodeStart, endNode);
        allPath.paths.ForEach(path => path //
            .ForEach(node => visitedPos.Add(node.Pos)));

        Console.WriteLine($"Pos. count {visitedPos.Count}");
    }

    private void Render(int sizeX, int sizeY, Vector v1, Vector v2) {
        for (int y = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                var v = new Vector(x, y);

                if (v.Equals(v1)) {
                    Console.Write("S");
                } else if (v.Equals(v2)) {
                    Console.Write("+");
                } else if (walls.Contains(v)) {
                    Console.Write("#");
                } else {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }

    private void addNodes(Vector pos) {
        if (walls.Contains(pos)) {
            return;
        }

        foreach (var step in ALL_STEPS) {
            var currentNode = new Node(pos, step);
            addNodes(currentNode, pos + step, step, 1);

            Vector newDir;
            // -
            newDir = step.RotateClockwise();
            addNodes(currentNode, pos, newDir, 1000);
            // -
            newDir = step.RotateAnticlockwise();
            addNodes(currentNode, pos, newDir, 1000);
        }
    }

    private void addNodes(Node currentNode, Vector nextPos, Vector dir, long cost) {
        var nextNode = new Node(nextPos, dir);
        if (walls.Contains(nextNode.Pos) || nextNode.Pos.X < 0 || nextNode.Pos.Y < 0) {
            return; // wall hit
        }

        // Console.WriteLine($"Node {currentNode}:");
        // Console.WriteLine($"  -> {nextNode} with cost {cost}");
        graph.AddEdge(currentNode, nextNode, cost);
    }
}

class Graph {
    public readonly Dictionary<Node, List<(Node neighbor, long cost)>> adjList;

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

    public Vector RotateClockwise() {
        return new Vector(Y, -X);
    }

    public Vector RotateAnticlockwise() {
        return new Vector(-Y, X);
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }
}