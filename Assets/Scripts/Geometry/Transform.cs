using System;
using System.Collections.Generic;

[Serializable]
public struct Transform : IEquatable<Transform> {

    public Point offset;
    public Direction rotation;

    public Transform(Point offset) {
        this.offset = offset;
        this.rotation = Direction.Right;
    }

    public Transform(Direction rotation) {
        this.offset = new Point();
        this.rotation = rotation;
    }

    public Transform(Point offset, Direction rotation) {
        this.offset = offset;
        this.rotation = rotation;
    }

    // Takes global coordinates to local coordinates.
    public Point Apply(Point point) {
        Point delta = point - this.offset;
        switch (this.rotation) {
            case Direction.Right:
                return new Point(delta.x, delta.y, delta.z);
            case Direction.Up:
                return new Point(delta.y, -delta.x - 1, delta.z);
            case Direction.Left:
                return new Point(-delta.x - 1, -delta.y - 1, delta.z);
            case Direction.Down:
                return new Point(-delta.y - 1, delta.x, delta.z);
            default:
                throw new InvalidOperationException("Invalid direction");
        }
    }

    public Direction Apply(Direction direction) {
        return (Direction)((-(int)this.rotation + (int)direction) % 4);
    }

    public Rectangle Apply(Rectangle rectangle) {
        Point lowerDelta = rectangle.lower - this.offset;
        Point upperDelta = rectangle.upper - this.offset;
        Point localLower;
        Point localUpper;
        switch (this.rotation) {
            case Direction.Right:
                localLower = new Point(lowerDelta.x, lowerDelta.y, lowerDelta.z);
                localUpper = new Point(upperDelta.x, upperDelta.y, upperDelta.z);
                break;
            case Direction.Up:
                localLower = new Point(lowerDelta.y, -lowerDelta.x, lowerDelta.z);
                localUpper = new Point(upperDelta.y, -upperDelta.x, upperDelta.z);
                break;
            case Direction.Left:
                localLower = new Point(-lowerDelta.x, -lowerDelta.y, lowerDelta.z);
                localUpper = new Point(-upperDelta.x, -upperDelta.y, upperDelta.z);
                break;
            case Direction.Down:
                localLower = new Point(-lowerDelta.y, lowerDelta.x, lowerDelta.z);
                localUpper = new Point(-upperDelta.y, upperDelta.x, upperDelta.z);
                break;
            default:
                throw new InvalidOperationException("Invalid direction");
        }
        return new Rectangle(localLower, localUpper);
    }

    // Takes local coordinates to global coordinates.
    public Point ApplyInverse(Point point) {
        Point localUnrotated;
        switch (this.rotation) {
            case Direction.Right:
                localUnrotated = new Point(point.x, point.y, point.z);
                break;
            case Direction.Up:
                localUnrotated = new Point(-point.y - 1, point.x, point.z);
                break;
            case Direction.Left:
                localUnrotated = new Point(-point.x - 1, -point.y - 1, point.z);
                break;
            case Direction.Down:
                localUnrotated = new Point(point.y, -point.x - 1, point.z);
                break;
            default:
                throw new InvalidOperationException("Invalid direction");
        }
        return localUnrotated + this.offset;
    }

    public Direction ApplyInverse(Direction direction) {
        return (Direction)(((int)this.rotation + (int)direction) % 4);
    }

    public Rectangle ApplyInverse(Rectangle rectangle) {
        Point lower = rectangle.lower;
        Point upper = rectangle.upper;
        Point localUnrotatedLower;
        Point localUnrotatedUpper;
        switch (this.rotation) {
            case Direction.Right:
                localUnrotatedLower = new Point(lower.x, lower.y, lower.z);
                localUnrotatedUpper = new Point(upper.x, upper.y, upper.z);
                break;
            case Direction.Up:
                localUnrotatedLower = new Point(-lower.y, lower.x, lower.z);
                localUnrotatedUpper = new Point(-upper.y, upper.x, upper.z);
                break;
            case Direction.Left:
                localUnrotatedLower = new Point(-lower.x, -lower.y, lower.z);
                localUnrotatedUpper = new Point(-upper.x, -upper.y, upper.z);
                break;
            case Direction.Down:
                localUnrotatedLower = new Point(lower.y, -lower.x, lower.z);
                localUnrotatedUpper = new Point(upper.y, -upper.x, upper.z);
                break;
            default:
                throw new InvalidOperationException("Invalid direction");
        }
        return new Rectangle(localUnrotatedLower + this.offset, localUnrotatedUpper + this.offset);
    }

    public override bool Equals(object obj) {
        if (obj is Transform) {
            return this.Equals((Transform)obj);
        }
        else {
            return false;
        }
    }
    public bool Equals(Transform other) {
        return this.offset.Equals(other.offset) && this.rotation.Equals(other.rotation);
    }

    public override int GetHashCode() {
        var hashCode = -1472577863;
        hashCode = hashCode * -1521134295 + EqualityComparer<Point>.Default.GetHashCode(offset);
        hashCode = hashCode * -1521134295 + rotation.GetHashCode();
        return hashCode;
    }
}
