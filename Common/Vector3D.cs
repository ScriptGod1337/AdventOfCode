using System.Numerics;

namespace Common;

public record Vector3D(long X, long Y, long Z) {
    public static Vector3D operator +(Vector3D p1, Vector3D p2) => new(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
    public static Vector3D operator -(Vector3D p1, Vector3D p2) => new(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
    public static Vector3D operator %(Vector3D p1, Vector3D p2) => new(p1.X % p2.X, p1.Y % p2.Y, p1.Z % p2.Z);
    public static Vector3D operator *(Vector3D p, long factor) => new(p.X * factor, p.Y * factor, p.Z * factor);
    public static Vector3D operator *(long factor, Vector3D p) => new(p.X * factor, p.Y * factor, p.Z * factor);
    public static Vector3D operator /(Vector3D p, long factor) => new(p.X / factor, p.Y / factor, p.Z / factor);
    public static Vector3D operator /(long factor, Vector3D p) => new(p.X / factor, p.Y / factor, p.Z / factor);
}

// public record Vector3D<T>(T X, T Y, T Z) 
//     where T: INumber<T> {
//     public static Vector3D operator +(Vector3D<T> p1, Vector3D<T> p2) => new(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
//     public static Vector3D operator -(Vector3D<T> p1, Vector3D<T> p2) => new(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
//     public static Vector3D operator %(Vector3D<T> p1, Vector3D<T> p2) => new(p1.X % p2.X, p1.Y % p2.Y, p1.Z % p2.Z);
//     public static Vector3D operator *(Vector3D<T> p, T factor) => new(p.X * factor, p.Y * factor, p.Z * factor);
//     public static Vector3D operator *(T factor, Vector3D<T> p) => new(p.X * factor, p.Y * factor, p.Z * factor);
//     public static Vector3D operator /(Vector3D<T> p, T factor) => new(p.X / factor, p.Y / factor, p.Z / factor);
//     public static Vector3D operator /(T factor, Vector3D<T> p) => new(p.X / factor, p.Y / factor, p.Z / factor);
// }