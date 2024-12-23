var graph = Graph.LoadFromFile("input.txt");

part1(graph);
part2(graph);

void part2(Graph graph) {
    var cliques = graph.BronKerbosch();
    var lookup = cliques.GroupBy(hs => hs.Count).ToDictionary(g => g.Key, g => g.ToList());

    var codeNodes = lookup[lookup.Keys.Max()];
    System.Diagnostics.Debug.Assert(codeNodes.Count == 1);
    Console.WriteLine($"Code: {string.Join(",", codeNodes[0].Order())}");
}

void part1(Graph graph) {
    int cycleLength = 3;
    var cycles = graph.FindCyclesOfLength(cycleLength);

    var allCyclesWithTNode = new HashSet<String>();
    if (cycles.Count > 0) {
        // Console.WriteLine($"Found {cycles.Count} cycles of length {cycleLength}:");
        foreach (var cycle in cycles) {
            // Console.WriteLine(string.Join(" -> ", cycle));
            if (cycle.Exists(x => x[0] == 't')) {
                allCyclesWithTNode.Add(string.Join(",", cycle.Order().ToList()));
            }
        }
    } else {
        Console.WriteLine($"No cycles of length {cycleLength} found.");
    }

    Console.WriteLine($"Result of t? nodes cycle {allCyclesWithTNode.Count}");
}

class Graph {
    private readonly Dictionary<string, List<string>> adjList;

    public Graph() {
        adjList = new Dictionary<string, List<string>>();
    }
    public void AddEdge(string src, string dest) {
        if (!adjList.ContainsKey(src))
            adjList[src] = new List<string>();

        if (!adjList.ContainsKey(dest))
            adjList[dest] = new List<string>();

        adjList[src].Add(dest);
        adjList[dest].Add(src); // Remove for directed graphs
    }

    public List<List<string>> FindCyclesOfLength(int k) {
        var allCycles = new List<List<string>>();
        var visited = new HashSet<string>();

        foreach (var node in adjList.Keys) {
            DFS(node, node, visited, k, 0, new List<string>(), allCycles);
        }

        return allCycles;
    }

    private void DFS(string start, string current, HashSet<string> visited, int targetLength, int currentLength, List<string> path, List<List<string>> allCycles) {
        visited.Add(current);
        path.Add(current);

        foreach (var neighbor in adjList[current]) {
            // If we reach the start node and the path length matches, it's a valid cycle
            if (neighbor == start && currentLength + 1 == targetLength) {
                allCycles.Add(new List<string>(path));
                continue;
            }

            // Continue DFS if the neighbor is unvisited and path length hasn't exceeded the target
            if (!visited.Contains(neighbor) && currentLength + 1 < targetLength) {
                DFS(start, neighbor, visited, targetLength, currentLength + 1, path, allCycles);
            }
        }

        // Backtrack
        visited.Remove(current);
        path.RemoveAt(path.Count - 1);
    }

    public List<HashSet<string>> BronKerbosch() {
        var cliques = new List<HashSet<string>>();
        var r = new HashSet<string>();
        var p = new HashSet<string>(adjList.Keys);
        var x = new HashSet<string>();

        BronKerboschRecursive(r, p, x, cliques);
        return cliques;
    }

    private void BronKerboschRecursive(HashSet<string> r, HashSet<string> p, HashSet<string> x, List<HashSet<string>> cliques) {
        if (p.Count == 0 && x.Count == 0) {
            // Maximal clique found
            cliques.Add(new HashSet<string>(r));
            return;
        }

        var pCopy = new HashSet<string>(p); // Create a copy to avoid modifying the set during iteration

        foreach (var v in pCopy) {
            var rNew = new HashSet<string>(r) { v };
            var pNew = new HashSet<string>(p.Intersect(adjList[v]));
            var xNew = new HashSet<string>(x.Intersect(adjList[v]));

            BronKerboschRecursive(rNew, pNew, xNew, cliques);

            p.Remove(v);
            x.Add(v);
        }
    }

    public List<List<string>> BFS(string start, int depthLimit) {
        var visited = new HashSet<string>();
        var queue = new Queue<(string Node, int Depth, List<string> Path)>();
        var paths = new List<List<string>>();

        // Start BFS from the given node with depth 0 and an initial path containing only the start node
        queue.Enqueue((start, 0, new List<string> { start }));
        // visited.Add(start);

        while (queue.Count > 0) {
            var (current, depth, path) = queue.Dequeue();

            // Add the current path to the result
            paths.Add(new List<string>(path));

            // Stop expanding beyond the depth limit
            if (depth >= depthLimit)
                continue;

            foreach (var neighbor in adjList[current]) {
                if (!visited.Contains(neighbor)) {
                    visited.Add(neighbor);

                    // Create a new path including the neighbor
                    var newPath = new List<string>(path) { neighbor };

                    queue.Enqueue((neighbor, depth + 1, newPath));
                }
            }
        }

        return paths;
    }


    public static Graph LoadFromFile(string filePath) {
        var graph = new Graph();
        foreach (var line in File.ReadAllLines(filePath)) {
            var nodes = line.Split('-');
            if (nodes.Length == 2) {
                graph.AddEdge(nodes[0], nodes[1]);
            }
        }
        return graph;
    }
}