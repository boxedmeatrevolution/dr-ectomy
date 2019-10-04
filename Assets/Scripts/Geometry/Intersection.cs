using System;

public static class Intersection {

    public static Point? Intersect(Point pointA, Point pointB) {
        if (Intersection.Contains(pointA, pointB)) {
            return pointA;
        }
        else {
            return null;
        }
    }
    public static Point? Intersect(Rectangle rect, Point point) {
        if (Intersection.Contains(rect, point)) {
            return point;
        }
        else {
            return null;
        }
    }
    public static Point? Intersect(Point point, Rectangle rect) {
        return Intersection.Intersect(rect, point);
    }
    public static Rectangle? Intersect(Rectangle rectA, Rectangle rectB) {
        Point newLower = new Point(
            Math.Max(rectA.lower.x, rectB.lower.x),
            Math.Max(rectA.lower.y, rectB.lower.y),
            Math.Max(rectA.lower.z, rectB.lower.z));
        Point newUpper = new Point(
            Math.Min(rectA.upper.x, rectB.upper.x),
            Math.Min(rectA.upper.y, rectB.upper.y),
            Math.Min(rectA.upper.z, rectB.upper.z));
        if (newLower.x <= newUpper.x && newLower.y <= newUpper.y && newLower.z <= newUpper.z) {
            return new Rectangle(newLower, newUpper);
        }
        else {
            return null;
        }
    }

    public static bool Contains(Point pointA, Point pointB) {
        return pointA.Equals(pointB);
    }
    public static bool Contains(Rectangle rect, Point point) {
        return point.x >= rect.lower.x && point.x < rect.upper.x &&
            point.y >= rect.lower.y && point.y < rect.upper.y &&
            point.z >= rect.lower.z && point.z < rect.upper.z;
    }
    public static bool Contains(Point point, Rectangle rect) {
        return Intersection.Contains(point, rect.lower) && Intersection.Contains(point, rect.upper);
    }
    public static bool Contains(Rectangle rectA, Rectangle rectB) {
        Point offset = new Point(1, 1, 1);
        return Intersection.Contains(rectA, rectB.lower) && Intersection.Contains(rectA, rectB.upper - offset);
    }
}
