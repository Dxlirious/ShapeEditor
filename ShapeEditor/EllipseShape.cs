using System;
using System.Drawing;

public class EllipseShape : Shape
{
    public Rectangle Bounds { get; set; }

    public override void Draw(Graphics graphics)
    {
        if (FillColor != Color.Transparent)
        {
            using (var brush = new SolidBrush(FillColor))
            {
                graphics.FillEllipse(brush, Bounds);
            }
        }

        using (var pen = new Pen(StrokeColor, StrokeThickness))
        {
            graphics.DrawEllipse(pen, Bounds);
        }
    }

    public override void UpdateShape(Point startPoint, Point endPoint)
    {
        Bounds = new Rectangle(
            Math.Min(startPoint.X, endPoint.X),
            Math.Min(startPoint.Y, endPoint.Y),
            Math.Abs(endPoint.X - startPoint.X),
            Math.Abs(endPoint.Y - startPoint.Y));
    }

    public override string Serialize()
    {
        return $"Ellipse|{Bounds.X},{Bounds.Y},{Bounds.Width},{Bounds.Height}|" +
               $"{StrokeColor.ToArgb()}|{StrokeThickness}|{FillColor.ToArgb()}";
    }

    public override Shape Deserialize(string data)
    {
        var parts = data.Split('|');
        var shape = new EllipseShape();

        var bounds = parts[1].Split(',');
        shape.Bounds = new Rectangle(
            int.Parse(bounds[0]),
            int.Parse(bounds[1]),
            int.Parse(bounds[2]),
            int.Parse(bounds[3]));

        shape.StrokeColor = Color.FromArgb(int.Parse(parts[2]));
        shape.StrokeThickness = int.Parse(parts[3]);
        shape.FillColor = Color.FromArgb(int.Parse(parts[4]));

        return shape;
    }
}