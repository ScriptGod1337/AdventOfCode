using Common;

namespace Common;

public record Polygon2D {
    private const double Eps = 1e-9;

    public IList<Vector2D> Points { get; init; }

    public Polygon2D(IEnumerable<Vector2D> points) {
        Points = [.. points];
    }

    // Ray casting point-in-polygon
    public bool ContainsPoint(Vector2D p) {
        bool inside = false;
        int count = Points.Count;

        for (int i = 0, j = count - 1; i < count; j = i++) {
            var pi = Points[i];
            var pj = Points[j];

            bool intersect = ((pi.Y > p.Y) != (pj.Y > p.Y)) &&
                             (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X);

            if (intersect) {
                inside = !inside;
            }
        }

        return inside;
    }

    public bool ContainsPointInclusive(Vector2D p) {
        bool inside = false;
        int count = Points.Count;

        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            var pi = Points[i];
            var pj = Points[j];

            // 1) boundary check: point on edge -> inside
            if (IsPointOnSegment(p, pj, pi))
                return true;

            // 2) standard ray casting
            bool intersect = ((pi.Y > p.Y) != (pj.Y > p.Y)) &&
                             (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X);

            if (intersect)
                inside = !inside;
        }

        return inside;
    }

    private static bool IsPointOnSegment(Vector2D p, Vector2D a, Vector2D b) {
        // cross product == 0  -> collinear
        double cross = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        if (Math.Abs(cross) > Eps)
            return false;

        // dot product to check within segment bounds
        double dot = (p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y);
        if (dot < -Eps) return false;

        double sqLen = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        if (dot - sqLen > Eps) return false;

        return true;
    }
    
    // Check that the whole rectangle is inside polygon
    public bool ContainsRectangle(VectorPlane2D rect) {
        var corners = rect.AllCorners;

        // 1. All corners inside
        foreach (var corner in corners) {
            if (!ContainsPointInclusive(corner)) {
                return false;
            }
        }

        // 2. No rectangle edge intersects any polygon edge
        for (int i = 0; i < corners.Length; i++) {
            var a = corners[i];
            var b = corners[(i + 1) % corners.Length];

            for (int j = 0; j < Points.Count; j++) {
                var c = Points[j];
                var d = Points[(j + 1) % Points.Count];

                if (SegmentsIntersect(a, b, c, d)) {
                    return false;
                }
            }
        }

        return true;
    }

    // Segment intersection (including collinear overlap if you want that as "intersection")
    private bool SegmentsIntersect(Vector2D a, Vector2D b, Vector2D c, Vector2D d) {
        double d1 = Cross(a, b, c);
        double d2 = Cross(a, b, d);
        double d3 = Cross(c, d, a);
        double d4 = Cross(c, d, b);

        return ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0));
    }
    
    // Cross product helper
    private double Cross(Vector2D a, Vector2D b, Vector2D c)
        => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    // public bool Contains(Vector2D p) =>
    //     IsInPolygon(p) || IsOnEdge(p);

    // public bool IsInPolygon(Vector2D p) {
    //     var inside = false;
    //     var n = Points.Count;
        
    //     for (int i = 0; i < n; i++) {
    //         int j = (i - 1 + n) % n; // previous index, wraps around
    //         var pi = Points[i];
    //         var pj = Points[j];

    //         bool intersect = ((pi.Y > p.Y) != (pj.Y > p.Y)) &&
    //                         (p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y) + pi.X);

    //         if (intersect) {
    //             inside = !inside;
    //         }
    //     }

    //     return inside;
    // }

    // public bool IsOnEdge(Vector2D p) {
    //     int n = Points.Count;

    //     for (int i = 0; i < n; i++) {
    //         var a = Points[i];
    //         var b = Points[(i + 1) % n];

    //         if (IsPointOnSegment(a, b, p)) {
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    // private bool IsPointOnSegment(Vector2D a, Vector2D b, Vector2D p) {
    //     // 1) Check collinearity via cross product
    //     double cross = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
    //     if (Math.Abs(cross) > Eps) {
    //         return false; // not collinear
    //     }

    //     // 2) Check p is within the bounding box of a-b
    //     double dot =
    //         (p.X - a.X) * (p.X - b.X) +
    //         (p.Y - a.Y) * (p.Y - b.Y);

    //     return dot <= Eps; // p is between a and b (inclusive)
    // }

    // public bool ContainsEdge(Vector2D a, Vector2D b) {
    //     // 1) endpoints must be inside or on the boundary
    //     if (!Contains(a) || !Contains(b)) {
    //         return false;
    //     }

    //     // 2) segment must not properly cross any polygon edge
    //     int n = Points.Count;
    //     for (int i = 0; i < n; i++) {
    //         var c = Points[i];
    //         var d = Points[(i + 1) % n];

    //         if (SegmentsProperlyIntersect(a, b, c, d)) {
    //             return false;
    //         }
    //     }

    //     return true;
    // }

    // private bool SegmentsProperlyIntersect(Vector2D p1, Vector2D p2, Vector2D q1, Vector2D q2) {
    //     double o1 = Orientation(p1, p2, q1);
    //     double o2 = Orientation(p1, p2, q2);
    //     double o3 = Orientation(q1, q2, p1);
    //     double o4 = Orientation(q1, q2, p2);

    //     // "Proper" intersection: they cross each other, not just touch, and not collinear
    //     return o1 * o2 < -Eps && o3 * o4 < -Eps;
    // }
    // private double Orientation(Vector2D a, Vector2D b, Vector2D c)
    //     => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
}