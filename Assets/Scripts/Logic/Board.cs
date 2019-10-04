using System;

[Serializable]
public class Board {

    public readonly Rectangle boundary;
    public readonly Type type;

    public Board(int width, int height, int depth, int zOffset, Type type) {
        if (width <= 0 || height <= 0 || depth <= 0) {
            throw new ArgumentOutOfRangeException("Board must have positive volume");
        }
        this.boundary = new Rectangle(new Point(0, 0, zOffset), new Point(width, height, zOffset + depth));
        this.type = type;
    }

    [Serializable]
    public enum Type {
        Organic,
        Metal,
    }

}
