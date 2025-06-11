using System;
using System.Drawing;

public class Line : Shape
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public override void Draw(Graphics graphics)
    {
        using (var pen = new Pen(StrokeColor, StrokeThickness))
        {
            graphics.DrawLine(pen, StartPoint, EndPoint);
        }
    }

    public override void UpdateShape(Point startPoint, Point endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public override string Serialize()
    {
        return $"Line|{StartPoint.X},{StartPoint.Y}|{EndPoint.X},{EndPoint.Y}|{StrokeColor.ToArgb()}|{StrokeThickness}|{FillColor.ToArgb()}";
    }

    public override Shape Deserialize(string data)
    {
        var parts = data.Split('|');
        var shape = new Line();

        var startCoords = parts[1].Split(',');
        shape.StartPoint = new Point(int.Parse(startCoords[0]), int.Parse(startCoords[1]));

        var endCoords = parts[2].Split(',');
        shape.EndPoint = new Point(int.Parse(endCoords[0]), int.Parse(endCoords[1]));

        shape.StrokeColor = Color.FromArgb(int.Parse(parts[3]));
        shape.StrokeThickness = int.Parse(parts[4]);
        shape.FillColor = Color.FromArgb(int.Parse(parts[5]));

        return shape;
    }
}