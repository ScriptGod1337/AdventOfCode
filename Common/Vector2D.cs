namespace Common;

public record Vector2D(long X, long Y) {
    public static Vector2D operator +(Vector2D p1, Vector2D p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Vector2D operator -(Vector2D p1, Vector2D p2) => new(p1.X - p2.X, p1.Y - p2.Y);
    public static Vector2D operator %(Vector2D p1, Vector2D p2) => new(p1.X % p2.X, p1.Y % p2.Y);
    public static Vector2D operator *(Vector2D p, long factor) => new(p.X * factor, p.Y * factor);
    public static Vector2D operator *(long factor, Vector2D p) => new(p.X * factor, p.Y * factor);
    public static Vector2D operator /(Vector2D p, long factor) => new(p.X / factor, p.Y / factor);
    public static Vector2D operator /(long factor, Vector2D p) => new(p.X / factor, p.Y / factor);
}