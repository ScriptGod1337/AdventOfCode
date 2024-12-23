Graph graph = Graph.LoadFromFile("input.txt");

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