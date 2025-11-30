// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;

var c = new Calculator();
// var result = 
c.GetPathWithHit("029");

class Calculator {

    private record WalkNode {

    }

    private List<Pad> layers = new List<Pad>();
    private Tree<string, string>? walkTree;

    public Calculator() {
        layers = new List<Pad>();
        layers.Add(new Pad(
            new char[,] {
                { '7', '8', '9' },
                { '4', '5', '6' },
                { '1', '2', '3' },
                { '\0', '0', 'A' }
                }
            ));

        for (var n = 0; n < 0; n++) {
            layers.Add(new Pad(
                new char[,] {
                    { '\0', '^', 'A' },
                    { '<', 'v', '>' }
                }
            ));
        }
    }

    public void GetPathWithHit(string sequence) {
        walkTree = new Tree<string, string>();
        walkTree.AddNode("A"); // root node

        GetPathWithHit(sequence, 'A', 0);
    }

    private IList<string> GetPathWithHit(string sequence, char startPos, int layerIdx) {
        if (layerIdx == layers.Count) {
            return new 
        }

        var toProcess = sequence + "A";
        for (var n = 0; n < toProcess.Length; n++) {
            var shortest = FindAllShortestRoutes(toProcess[n], (n > 0) ? toProcess[n - 1] : startPos, layerIdx);
            cost += pCost;
            Console.WriteLine(p + "A");
        }

        return cost;
    }

    private List<string> FindAllShortestRoutes(char c, char startPos, int layerIdx) {
        var result = List<string>();
        var allPath = layers[layerIdx].FindAllShortestRoutes(c, startPos);
        foreach (var p in allPath) {
            var nextSeq = new string(p.ToArray());
            var nextPath = GetPathWithHit(nextSeq, startPos, layerIdx + 1);
            result.Add(nextPath);
        }
    }

    private (string, ulong) FindShortsPath(char c, char startPos, int layerIdx) {
        var allPath = layers[layerIdx].FindAllShortestRoutes(c, startPos);

        var minCost = ulong.MaxValue;
        string? minPath = null;
        foreach (var p in allPath) {
            var nextSeq = new string(p.ToArray());
            var pathCost = GetPathWithHit(nextSeq, startPos, layerIdx + 1);
            if (minCost > pathCost) {
                minCost = pathCost;
                minPath = nextSeq;
            }
        }

        System.Diagnostics.Debug.Assert(minCost != long.MaxValue);
        System.Diagnostics.Debug.Assert(minPath != null);

        return (minPath, minCost);
    }
}

class Pad {
    private Tree<char, char> padTree = new Tree<char, char>();

    public Pad(char[,] matrix) {
        padTree.InitializeTreeFromMatrix(matrix, '<', '>', '^', 'v');
        // padTree.DisplayTree();
    }

    public List<List<char>> FindAllShortestRoutes(char c, char startPos) {
        return padTree.FindAllShortestRoutes(startPos, c);
    }
}

#region Tree
public class TreeNode<TId, TAttribute> {
    public TId Id { get; }
    public List<Edge<TId, TAttribute>> Edges { get; }

    public TreeNode(TId id) {
        Id = id;
        Edges = new List<Edge<TId, TAttribute>>();
    }

    public void AddEdge(TId targetId, TAttribute attribute) {
        Edges.Add(new Edge<TId, TAttribute>(Id, targetId, attribute));
    }
}

public class Edge<TId, TAttribute> {
    public TId SourceId { get; }
    public TId TargetId { get; }
    public TAttribute Attribute { get; }

    public Edge(TId sourceId, TId targetId, TAttribute attribute) {
        SourceId = sourceId;
        TargetId = targetId;
        Attribute = attribute;
    }
}

public class Tree<TId, TAttribute> {
    private Dictionary<TId, TreeNode<TId, TAttribute>> nodes;

    public Tree() {
        nodes = new Dictionary<TId, TreeNode<TId, TAttribute>>();
    }

    public void AddNode(TId id) {
        if (!nodes.ContainsKey(id)) {
            nodes[id] = new TreeNode<TId, TAttribute>(id);
        }
    }

    public void AddEdge(TId sourceId, TId targetId, TAttribute attribute) {
        if (!nodes.ContainsKey(sourceId)) {
            AddNode(sourceId);
        }
        if (!nodes.ContainsKey(targetId)) {
            AddNode(targetId);
        }

        nodes[sourceId].AddEdge(targetId, attribute);
    }

    public void InitializeTreeFromMatrix(TId[,] matrix, TAttribute left, TAttribute right, TAttribute up, TAttribute down) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                TId currentId = matrix[i, j];

                if (EqualityComparer<TId>.Default.Equals(currentId, default)) continue; // Skip empty cells

                // Add connections
                if (j > 0 && !EqualityComparer<TId>.Default.Equals(matrix[i, j - 1], default)) // Left
                {
                    AddEdge(currentId, matrix[i, j - 1], left);
                }
                if (j < cols - 1 && !EqualityComparer<TId>.Default.Equals(matrix[i, j + 1], default)) // Right
                {
                    AddEdge(currentId, matrix[i, j + 1], right);
                }
                if (i > 0 && !EqualityComparer<TId>.Default.Equals(matrix[i - 1, j], default)) // Up
                {
                    AddEdge(currentId, matrix[i - 1, j], up);
                }
                if (i < rows - 1 && !EqualityComparer<TId>.Default.Equals(matrix[i + 1, j], default)) // Down
                {
                    AddEdge(currentId, matrix[i + 1, j], down);
                }
            }
        }
    }

    public List<List<TAttribute>> FindAllShortestRoutes(TId startId, TId endId) {
        if (!nodes.ContainsKey(startId) || !nodes.ContainsKey(endId))
            return new List<List<TAttribute>>();

        var visited = new HashSet<TId>();
        var queue = new Queue<(TId currentId, List<TAttribute> path)>();
        var shortestPaths = new List<List<TAttribute>>();
        int shortestLength = int.MaxValue;

        queue.Enqueue((startId, new List<TAttribute>()));

        while (queue.Count > 0) {
            var (currentId, path) = queue.Dequeue();

            if (visited.Contains(currentId) && path.Count > shortestLength)
                continue;

            visited.Add(currentId);

            foreach (var edge in nodes[currentId].Edges) {
                var newPath = new List<TAttribute>(path) { edge.Attribute };

                if (EqualityComparer<TId>.Default.Equals(edge.TargetId, endId)) {
                    if (newPath.Count < shortestLength) {
                        shortestPaths.Clear();
                        shortestLength = newPath.Count;
                    }

                    if (newPath.Count == shortestLength) {
                        shortestPaths.Add(newPath);
                    }
                } else if (newPath.Count <= shortestLength) {
                    queue.Enqueue((edge.TargetId, newPath));
                }
            }
        }

        return shortestPaths;
    }

    public void DisplayTree() {
        foreach (var node in nodes.Values) {
            Console.WriteLine($"Node {node.Id}:");
            foreach (var edge in node.Edges) {
                Console.WriteLine($"  -> {edge.TargetId} ({edge.Attribute})");
            }
        }
    }

}
#endregion