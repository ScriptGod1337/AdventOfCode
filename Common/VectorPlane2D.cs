namespace Common;

public record VectorPlane2D {
    public Vector2D[] AllCorners { get; init; }

    public Vector2D First { get => AllCorners[firstIndex]; }
    public Vector2D Second { get => AllCorners[secondIndex]; }

    private readonly int firstIndex;
    private readonly int secondIndex;

    public VectorPlane2D(Vector2D first, Vector2D second) {
        var minX = Math.Min(first.X, second.X);
        var maxX = Math.Max(first.X, second.X);
        var minY = Math.Min(first.Y, second.Y);
        var maxY = Math.Max(first.Y, second.Y);

        AllCorners = [
            new Vector2D(minX, minY), // bottom-left
            new Vector2D(maxX, minY), // bottom-right
            new Vector2D(maxX, maxY), // top-right
            new Vector2D(minX, maxY), // top-left
        ];

        firstIndex = Array.IndexOf(AllCorners, first);
        secondIndex = Array.IndexOf(AllCorners, second);
    }

    public bool Contains(Vector2D p) {
        var minX = AllCorners[0].X; // bottom-left
        var maxX = AllCorners[1].X; // bottom-right
        var minY = AllCorners[0].Y; // bottom-left
        var maxY = AllCorners[3].Y; // top-left

        return p.X >= minX && p.X <= maxX
            && p.Y >= minY && p.Y <= maxY;
    }

    public long Size {
        get => (AllCorners[1].X - AllCorners[0].X + 1) * (AllCorners[3].Y - AllCorners[0].Y + 1);
    }

    override public string ToString()
        => $"Plane2D(First: {First} First: {Second} AllCorners: {string.Join(", ", AllCorners.ToList())} )";
}