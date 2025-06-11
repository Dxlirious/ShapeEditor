using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

public class DrawingHistory
{
    private Stack<List<Shape>> undoStack = new Stack<List<Shape>>();
    private Stack<List<Shape>> redoStack = new Stack<List<Shape>>();
    private List<Shape> currentShapes = new List<Shape>();

    public void AddShape(Shape shape)
    {
        redoStack.Clear();
        undoStack.Push(new List<Shape>(currentShapes));
        currentShapes.Add(shape);
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            redoStack.Push(new List<Shape>(currentShapes));
            currentShapes = undoStack.Pop();
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            undoStack.Push(new List<Shape>(currentShapes));
            currentShapes = redoStack.Pop();
        }
    }

    public void DrawAll(Graphics graphics)
    {
        foreach (var shape in currentShapes)
        {
            shape.Draw(graphics);
        }
    }

    public string Serialize()
    {
        var sb = new StringBuilder();
        foreach (var shape in currentShapes)
        {
            sb.AppendLine(shape.Serialize());
        }
        return sb.ToString();
    }

    public void Deserialize(string data)
    {
        currentShapes.Clear();
        undoStack.Clear();
        redoStack.Clear();

        var lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var type = line.Split('|')[0];
            var shape = ShapeFactory.CreateShape(type);
            currentShapes.Add(shape.Deserialize(line));
        }
    }
}