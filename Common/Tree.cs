namespace Common;
public class Tree<TNode, TStep>
    where TNode : notnull {
    private readonly Dictionary<TNode, HashSet<(TNode Next, TStep Edge)>> adjList = [];

    public void AddNode(TNode from) {
        if (!adjList.ContainsKey(from)) {
            adjList[from] = [];
        }
    }
    public bool AddEdge(TNode from, TNode to, TStep Edge) {
        AddNode(from);
        AddNode(to);

        return adjList[from].Add((to, Edge));
    }

    // Returns true if a path exists. Path includes start and goal.
    public bool TryShortestPath(
        TNode start,
        TNode goal,
        Func<TStep, double> weightSelector,
        out List<TNode> path,
        out double totalCost) {
        path = [];
        totalCost = double.PositiveInfinity;

        if (!adjList.ContainsKey(start) || !adjList.ContainsKey(goal)) {
            return false;
        }

        var dist = new Dictionary<TNode, double>();
        var prev = new Dictionary<TNode, TNode>();
        var pq = new PriorityQueue<TNode, double>();

        dist[start] = 0.0;
        pq.Enqueue(start, 0.0);

        while (pq.Count > 0) {
            var u = pq.Dequeue();
            var du = dist[u];

            if (EqualityComparer<TNode>.Default.Equals(u, goal)) {
                break;
            }

            foreach (var (v, edge) in adjList[u]) {
                var w = weightSelector(edge);
                if (w < 0) {
                    throw new ArgumentOutOfRangeException(nameof(weightSelector), "Edge weights must be non-negative.");
                }

                var alt = du + w;
                if (!dist.TryGetValue(v, out var dv) || alt < dv) {
                    dist[v] = alt;
                    prev[v] = u;
                    pq.Enqueue(v, alt);
                }
            }
        }

        if (!dist.TryGetValue(goal, out totalCost)) {
            return false;
        }

        // Reconstruct path
        var cur = goal;
        path.Add(cur);
        while (!EqualityComparer<TNode>.Default.Equals(cur, start)) {
            if (!prev.TryGetValue(cur, out var p)) {
                return false; // safety
            }
            cur = p;
            path.Add(cur);
        }
        path.Reverse();
        return true;
    }
}