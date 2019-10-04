using System;

[Serializable]
public struct Point : IEquatable<Point> {

    public int x;
    public int y;
    public int z;

    public Point(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Point operator +(Point lhs, Point rhs) {
        return new Point(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    }

    public static Point operator -(Point lhs, Point rhs) {
        return new Point(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
    }

    public static Point operator -(Point point) {
        return new Point(-point.x, -point.y, -point.z);
    }

    public override bool Equals(object obj) {
        if (obj is Point) {
            return this.Equals((Point)obj);
        }
        else {
            return false;
        }
    }
    public bool Equals(Point other) {
        return this.x == other.x && this.y == other.y && this.z == other.z;
    }

    public override int GetHashCode() {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }

}
