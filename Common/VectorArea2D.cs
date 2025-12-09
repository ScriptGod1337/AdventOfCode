namespace Common;

public record VectorArea2D(Vector2D a, Vector2D b) {
    public long Size() =>
        (Math.Abs(a.X - b.X) + 1) * (Math.Abs(a.Y - b.Y) + 1);
}