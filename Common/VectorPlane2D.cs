using System.Dynamic;
using System.Text.RegularExpressions;

namespace Common;

public record VectorPlane2D {
    public Vector2D[] AllCorners { get; init; }

    public Vector2D First { get => AllCorners[0]; }
    public Vector2D Second { get => AllCorners[1]; }

    public VectorPlane2D(Vector2D first, Vector2D second) {
        this.AllCorners = [
            first,
            second,
            new Vector2D(Math.Min(first.X, second.X), Math.Min(first.Y, second.Y)),
            new Vector2D(Math.Max(first.X, second.X), Math.Max(first.Y, second.Y)),
        ];
    }

    public long Size {
        get => (Math.Abs(First.X - Second.X) + 1) * (Math.Abs(First.Y - Second.Y) + 1);
    }
}