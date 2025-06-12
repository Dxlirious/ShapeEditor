using System;
using System.Drawing;

namespace TrapezoidPlugin
{
    public class TrapezoidShape : Shape
    {
        public Point[] Points { get; set; } = new Point[4];

        public override void Draw(Graphics graphics)
        {
            if (FillColor != Color.Transparent)
            {
                using (var brush = new SolidBrush(FillColor))
                {
                    graphics.FillPolygon(brush, Points);
                }
            }

            using (var pen = new Pen(StrokeColor, StrokeThickness))
            {
                graphics.DrawPolygon(pen, Points);
            }
        }

        public override void UpdateShape(Point startPoint, Point endPoint)
        {
            int width = Math.Abs(endPoint.X - startPoint.X);
            int height = Math.Abs(endPoint.Y - startPoint.Y);
            int offset = width / 3;

            Points[0] = new Point(startPoint.X + (startPoint.X < endPoint.X ? offset : 0), startPoint.Y);
            Points[1] = new Point(startPoint.X + (startPoint.X < endPoint.X ? width - offset : width), startPoint.Y);
            Points[2] = new Point(startPoint.X + (startPoint.X < endPoint.X ? width : 0), endPoint.Y);
            Points[3] = new Point(startPoint.X + (startPoint.X < endPoint.X ? 0 : width), endPoint.Y);
        }

        public override string Serialize()
        {
            return $"Trapezoid|{Points[0].X},{Points[0].Y}|{Points[1].X},{Points[1].Y}|" +
                   $"{Points[2].X},{Points[2].Y}|{Points[3].X},{Points[3].Y}|" +
                   $"{StrokeColor.ToArgb()}|{StrokeThickness}|{FillColor.ToArgb()}";
        }

        public override Shape Deserialize(string data)
        {
            var parts = data.Split('|');
            var shape = new TrapezoidShape();

            for (int i = 0; i < 4; i++)
            {
                var coords = parts[i + 1].Split(',');
                shape.Points[i] = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
            }

            shape.StrokeColor = Color.FromArgb(int.Parse(parts[5]));
            shape.StrokeThickness = int.Parse(parts[6]);
            shape.FillColor = Color.FromArgb(int.Parse(parts[7]));

            return shape;
        }
    }
}