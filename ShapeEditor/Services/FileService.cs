using System.IO;
using ShapeEditor.Core;

public class FileService
{
    private readonly DrawingHistory _drawingHistory;

    public FileService(DrawingHistory drawingHistory)
    {
        _drawingHistory = drawingHistory;
    }

    public void SaveToFile(string filePath)
    {
        File.WriteAllText(filePath, _drawingHistory.Serialize());
    }

    public void LoadFromFile(string filePath)
    {
        _drawingHistory.Deserialize(File.ReadAllText(filePath));
    }
}