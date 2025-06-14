using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ShapeEditor.Core;

public class Polygon : Shape
{
    public List<Point> Points { get; set; } = new List<Point>();
    private bool isCompleted = false;

    public override void Draw(Graphics graphics)
    {
        if (Points.Count < 2) return;

        if (FillColor != Color.Transparent)
        {
            using (var brush = new SolidBrush(FillColor))
            {
                graphics.FillPolygon(brush, Points.ToArray());
            }
        }

        using (var pen = new Pen(StrokeColor, StrokeThickness))
        {
            graphics.DrawPolygon(pen, Points.ToArray());
        }
    }

    public override void UpdateShape(Point startPoint, Point endPoint)
    {
        if (!isCompleted)
        {
            if (Points.Count == 0)
            {
                Points.Add(startPoint);
            }
            Points.Add(endPoint);
        }
    }

    public void CompleteShape()
    {
        isCompleted = true;
        if (Points.Count > 2)
        {
            Points.Add(Points[0]);
        }
    }

    public override string Serialize()
    {
        var pointsStr = string.Join(";", Points.Select(p => $"{p.X},{p.Y}"));
        return $"Polygon|{pointsStr}|{StrokeColor.ToArgb()}|{StrokeThickness}|" +
               $"{FillColor.ToArgb()}|{isCompleted}";
    }

    public override Shape Deserialize(string data)
    {
        var parts = data.Split('|');
        var shape = new Polygon();

        var points = parts[1].Split(';')
            .Select(s => s.Split(','))
            .Select(c => new Point(int.Parse(c[0]), int.Parse(c[1])))
            .ToList();
        shape.Points = points;

        shape.StrokeColor = Color.FromArgb(int.Parse(parts[2]));
        shape.StrokeThickness = int.Parse(parts[3]);
        shape.FillColor = Color.FromArgb(int.Parse(parts[4]));
        shape.isCompleted = bool.Parse(parts[5]);

        return shape;
    }
}