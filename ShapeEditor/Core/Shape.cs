using System.Drawing;

namespace ShapeEditor.Core
{

    public abstract class Shape
    {
        public Color StrokeColor { get; set; } = Color.Black;
        public int StrokeThickness { get; set; } = 1;
        public Color FillColor { get; set; } = Color.Transparent;

        public abstract void Draw(Graphics graphics);
        public abstract void UpdateShape(Point startPoint, Point endPoint);
        public abstract string Serialize();
        public abstract Shape Deserialize(string data);
    }
}