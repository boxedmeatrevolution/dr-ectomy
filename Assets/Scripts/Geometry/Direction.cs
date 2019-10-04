using System;

[Serializable]
public enum Direction {
    Right = 0,
    Up = 1,
    Left = 2,
    Down = 3,
}

public static class DirectionMethods {
    public static Point GetUnitVector(this Direction direction) {
        switch (direction) {
            case Direction.Right:
                return new Point(1, 0, 0);
            case Direction.Up:
                return new Point(0, 1, 0);
            case Direction.Left:
                return new Point(-1, 0, 0);
            case Direction.Down:
                return new Point(0, -1, 0);
            default:
                throw new InvalidOperationException("Invalid direction");
        }
    }

    // Get relative directions.
    public static Direction Counterclockwise(this Direction direction) {
        return (Direction)(((int)direction + 1) % 4);
    }
    public static Direction Opposite(this Direction direction) {
        return (Direction)(((int)direction + 2) % 4);
    }
    public static Direction Clockwise(this Direction direction) {
        return (Direction)(((int)direction + 3) % 4);
    }
}
