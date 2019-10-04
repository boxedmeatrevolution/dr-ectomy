using System;
using System.Collections.Generic;

[Serializable]
public struct Rectangle : IEquatable<Rectangle> {

    public readonly Point lower;
    public readonly Point upper;

    public Rectangle(Point lower, Point upper) {
        this.lower = new Point(Math.Min(lower.x, upper.x), Math.Min(lower.y, upper.y), Math.Min(lower.z, upper.z));
        this.upper = new Point(Math.Max(lower.x, upper.x), Math.Max(lower.y, upper.y), Math.Max(lower.z, upper.z));
    }

    public Point GetDimensions() {
        return this.upper - this.lower;
    }

    public override bool Equals(object obj) {
        if (obj is Rectangle) {
            return this.Equals((Rectangle)obj);
        }
        else {
            return false;
        }
    }
    public bool Equals(Rectangle other) {
        return this.lower.Equals(other.lower) && this.upper.Equals(other.upper);
    }

    public override int GetHashCode() {
        var hashCode = 579202577;
        hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(lower);
        hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(upper);
        return hashCode;
    }

}
