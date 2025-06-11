using System.Collections.Generic;
using System;

public static class ShapeFactory
{
    private static Dictionary<string, Func<Shape>> shapeCreators = new Dictionary<string, Func<Shape>>()
    {
        { "Line", () => new Line() },
        { "Rectangle", () => new RectangleShape() },
        { "Ellipse", () => new EllipseShape() },
        { "Polyline", () => new Polyline() },
        { "Polygon", () => new Polygon() }
    };

    public static void RegisterShape(string shapeName, Func<Shape> creator)
    {
        shapeCreators[shapeName] = creator;
    }

    public static Shape CreateShape(string shapeName)
    {
        if (shapeCreators.ContainsKey(shapeName))
        {
            return shapeCreators[shapeName]();
        }
        throw new ArgumentException($"Unknown shape type: {shapeName}");
    }
}