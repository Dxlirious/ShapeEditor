using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShapeEditor.Core;

namespace ShapeEditor.UI
{
    public class DrawingCanvas : PictureBox
    {
        private readonly DrawingHistory _drawingHistory;
        private readonly CanvasService _canvasService;
        private readonly FileService _fileService;
        private readonly PluginService _pluginService;

        private Point _lastPoint;
        private bool _isDrawing;

        public Color StrokeColor { get; set; } = Color.Black;
        public Color FillColor { get; set; } = Color.Transparent;
        public int StrokeThickness { get; set; } = 2;
        public string CurrentShapeType { get; set; } = "Line";

        public DrawingCanvas()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            DoubleBuffered = true;

            _drawingHistory = new DrawingHistory();
            _canvasService = new CanvasService(_drawingHistory);
            _fileService = new FileService(_drawingHistory);
            _pluginService = new PluginService();

            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            DoubleClick += OnDoubleClick;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = true;
                _lastPoint = e.Location;

                _canvasService.StartDrawing(e.Location, CurrentShapeType, StrokeColor, FillColor, StrokeThickness);
                Invalidate();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing && e.Button == MouseButtons.Left)
            {
                _canvasService.UpdateDrawing(_lastPoint, e.Location);
                _lastPoint = e.Location;
                Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_isDrawing && e.Button == MouseButtons.Left)
            {
                _canvasService.CompleteDrawing();
                _isDrawing = false;
                Invalidate();
            }
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            _canvasService.HandleDoubleClick();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _canvasService.DrawAll(e.Graphics);
        }

        public void SaveDrawing(string filePath)
        {
            _fileService.SaveToFile(filePath);
        }

        public void LoadDrawing(string filePath)
        {
            _fileService.LoadFromFile(filePath);
            Invalidate();
        }

        public void LoadPlugin(string pluginPath, ToolStrip toolStrip)
        {
            try
            {
                if (!File.Exists(pluginPath))
                {
                    MessageBox.Show("Файл плагина не найден");
                    return;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(pluginPath));
                File.Copy(pluginPath, tempPath, true);

                _pluginService.LoadPlugin(tempPath, toolStrip, shapeType =>
                {
                    this.CurrentShapeType = shapeType;
                    UpdateToolStripSelection(toolStrip, shapeType);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки плагина: {ex.Message}");
            }
        }

        private void UpdateToolStripSelection(ToolStrip toolStrip, string shapeType)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item is ToolStripButton button)
                {
                    button.Checked = (button.Text == shapeType);
                }
            }
        }

        public void Undo()
        {
            _drawingHistory.Undo();
            Invalidate();
        }

        public void Redo()
        {
            _drawingHistory.Redo();
            Invalidate();
        }

        public void SetShapeType(string shapeType)
        {
            CurrentShapeType = shapeType;
        }

        public void SetStrokeColor(Color color)
        {
            StrokeColor = color;
        }

        public void SetFillColor(Color color)
        {
            FillColor = color;
        }

        public void SetStrokeThickness(int thickness)
        {
            StrokeThickness = thickness;
        }
    }
}