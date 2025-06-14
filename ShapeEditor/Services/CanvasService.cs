using System.Drawing;
using ShapeEditor.Core;

public class CanvasService
{
    private readonly DrawingHistory _drawingHistory;
    private Shape _currentShape;

    public CanvasService(DrawingHistory drawingHistory)
    {
        _drawingHistory = drawingHistory;
    }

    public void StartDrawing(Point startPoint, string shapeType, Color strokeColor, Color fillColor, int thickness)
    {
        _currentShape = ShapeFactory.CreateShape(shapeType);
        _currentShape.StrokeColor = strokeColor;
        _currentShape.StrokeThickness = thickness;
        _currentShape.FillColor = fillColor;
        _currentShape.UpdateShape(startPoint, startPoint);
    }

    public void UpdateDrawing(Point startPoint, Point currentPoint)
    {
        if (_currentShape != null)
        {
            _currentShape.UpdateShape(startPoint, currentPoint);
        }
    }

    public void CompleteDrawing()
    {
        if (_currentShape != null)
        {
            _drawingHistory.AddShape(_currentShape);
            _currentShape = null;
        }
    }

    public void DrawAll(Graphics graphics)
    {
        _drawingHistory.DrawAll(graphics);
        _currentShape?.Draw(graphics);
    }

    public void HandleDoubleClick()
    {
        if (_currentShape is Polyline polyline)
        {
            polyline.CompleteShape();
            CompleteDrawing();
        }
        else if (_currentShape is Polygon polygon)
        {
            polygon.CompleteShape();
            CompleteDrawing();
        }
    }
}