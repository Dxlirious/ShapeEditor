using System;
using System.Drawing;

public class RectangleShape : Shape
{
    public Rectangle Rect { get; set; }

    public override void Draw(Graphics graphics)
    {
        if (FillColor != Color.Transparent)
        {
            using (var brush = new SolidBrush(FillColor))
            {
                graphics.FillRectangle(brush, Rect);
            }
        }

        using (var pen = new Pen(StrokeColor, StrokeThickness))
        {
            graphics.DrawRectangle(pen, Rect);
        }
    }

    public override void UpdateShape(Point startPoint, Point endPoint)
    {
        Rect = new Rectangle(
            Math.Min(startPoint.X, endPoint.X),
            Math.Min(startPoint.Y, endPoint.Y),
            Math.Abs(endPoint.X - startPoint.X),
            Math.Abs(endPoint.Y - startPoint.Y));
    }

    public override string Serialize()
    {
        return $"Rectangle|{Rect.X},{Rect.Y},{Rect.Width},{Rect.Height}|" +
               $"{StrokeColor.ToArgb()}|{StrokeThickness}|{FillColor.ToArgb()}";
    }

    public override Shape Deserialize(string data)
    {
        var parts = data.Split('|');
        var shape = new RectangleShape();

        var rectParts = parts[1].Split(',');
        shape.Rect = new Rectangle(
            int.Parse(rectParts[0]),
            int.Parse(rectParts[1]),
            int.Parse(rectParts[2]),
            int.Parse(rectParts[3]));

        shape.StrokeColor = Color.FromArgb(int.Parse(parts[2]));
        shape.StrokeThickness = int.Parse(parts[3]);
        shape.FillColor = Color.FromArgb(int.Parse(parts[4]));

        return shape;
    }
}