using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ShapeEditor
{
    public partial class MainForm : Form
    {
        private ToolStrip mainToolStrip;
        private ComboBox thicknessComboBox;
        private ToolStripButton lineButton, rectangleButton, ellipseButton, polylineButton, polygonButton;
        private ToolStripButton undoButton, redoButton, saveButton, loadButton, pluginButton;
        private ColorDialog colorDialog = new ColorDialog();
        private Bitmap drawingBitmap;

        private DrawingHistory drawingHistory = new DrawingHistory();
        private Shape currentShape;
        private Point startPoint;
        private string currentShapeType = "Line";
        private bool isDrawing = false;

        public MainForm()
        {
            InitializeComponent();
            SetupToolStrip();
            InitializeEventHandlers();

            drawingBitmap = new Bitmap(pictureBoxCanvas.Width, pictureBoxCanvas.Height);
            ClearCanvas();
        }

        private void SetupToolStrip()
        {
            mainToolStrip = new ToolStrip { Dock = DockStyle.Top };
            this.Controls.Add(mainToolStrip);

            thicknessComboBox = new ComboBox();
            thicknessComboBox.Items.AddRange(new object[] { 1, 2, 3, 5, 8 });
            thicknessComboBox.SelectedIndex = 0;

            lineButton = CreateToolButton("Line");
            rectangleButton = CreateToolButton("Rectangle");
            ellipseButton = CreateToolButton("Ellipse");
            polylineButton = CreateToolButton("Polyline");
            polygonButton = CreateToolButton("Polygon");

            undoButton = CreateToolButton("Undo");
            redoButton = CreateToolButton("Redo");
            saveButton = CreateToolButton("Save");
            loadButton = CreateToolButton("Load");
            pluginButton = CreateToolButton("Load Plugin");

            var colorButton = CreateToolButton("Color");
            var thicknessItem = new ToolStripControlHost(thicknessComboBox);

            mainToolStrip.Items.AddRange(new ToolStripItem[] {
                lineButton, rectangleButton, ellipseButton, polylineButton, polygonButton,
                new ToolStripSeparator(),
                undoButton, redoButton,
                new ToolStripSeparator(),
                saveButton, loadButton,
                new ToolStripSeparator(),
                pluginButton,
                thicknessItem,
                colorButton
            });
        }

        private ToolStripButton CreateToolButton(string text)
        {
            return new ToolStripButton(text) { DisplayStyle = ToolStripItemDisplayStyle.Text };
        }

        private void InitializeEventHandlers()
        {
            lineButton.Click += (s, e) => currentShapeType = "Line";
            rectangleButton.Click += (s, e) => currentShapeType = "Rectangle";
            ellipseButton.Click += (s, e) => currentShapeType = "Ellipse";
            polylineButton.Click += PolylineButton_Click;
            polygonButton.Click += PolygonButton_Click;

            undoButton.Click += UndoButton_Click;
            redoButton.Click += RedoButton_Click;
            saveButton.Click += SaveButton_Click;
            loadButton.Click += LoadButton_Click;
            pluginButton.Click += PluginButton_Click;

            mainToolStrip.Items[mainToolStrip.Items.Count - 1].Click += ColorButton_Click;

            pictureBoxCanvas.MouseDown += Canvas_MouseDown;
            pictureBoxCanvas.MouseMove += Canvas_MouseMove;
            pictureBoxCanvas.MouseUp += Canvas_MouseUp;
            pictureBoxCanvas.DoubleClick += Canvas_DoubleClick;
            pictureBoxCanvas.Paint += Canvas_Paint;
        }

        private void PolylineButton_Click(object sender, EventArgs e)
        {
            currentShapeType = "Polyline";
            StartPolyDrawing();
        }

        private void PolygonButton_Click(object sender, EventArgs e)
        {
            currentShapeType = "Polygon";
            StartPolyDrawing();
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            drawingHistory.Undo();
            RedrawCanvas();
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            drawingHistory.Redo();
            RedrawCanvas();
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK) { }
        }

        private void StartPolyDrawing()
        {
            if (currentShape is Polyline || currentShape is Polygon) return;

            currentShape = ShapeFactory.CreateShape(currentShapeType);
            currentShape.StrokeColor = colorDialog.Color;
            currentShape.StrokeThickness = (int)thicknessComboBox.SelectedItem;
            currentShape.FillColor = colorDialog.Color;
        }

        private void PluginButton_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Plugin files (*.dll)|*.dll";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(openDialog.FileName);
                        bool pluginLoaded = false;

                        foreach (var type in assembly.GetTypes())
                        {
                            if (typeof(Shape).IsAssignableFrom(type) && !type.IsAbstract)
                            {
                                MessageBox.Show($"Найден тип: {type.Name}", "Отладка");

                                ShapeFactory.RegisterShape(type.Name, () => (Shape)Activator.CreateInstance(type));

                                var button = new ToolStripButton(type.Name)
                                {
                                    DisplayStyle = ToolStripItemDisplayStyle.Text,
                                    Tag = openDialog.FileName
                                };
                                button.Click += (s, ev) => currentShapeType = type.Name;

                                int insertIndex = mainToolStrip.Items.Count - 3;
                                mainToolStrip.Items.Insert(insertIndex, button);

                                pluginLoaded = true;
                            }
                        }

                        if (!pluginLoaded)
                            MessageBox.Show("Не найдены классы фигур в плагине", "Ошибка");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                    }
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currentShape is Polyline || currentShape is Polygon)
                {
                    currentShape.UpdateShape(e.Location, e.Location);
                    RedrawCanvas();
                }
                else
                {
                    isDrawing = true;
                    startPoint = e.Location;
                    currentShape = ShapeFactory.CreateShape(currentShapeType);
                    currentShape.StrokeColor = colorDialog.Color;
                    currentShape.StrokeThickness = (int)thicknessComboBox.SelectedItem;
                    currentShape.FillColor = colorDialog.Color;
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            if (currentShape != null)
            {
                currentShape.UpdateShape(startPoint, e.Location);
                RedrawCanvas();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            isDrawing = false;
            if (currentShape != null)
            {
                drawingHistory.AddShape(currentShape);
                currentShape = null;
                RedrawCanvas();
            }
        }

        private void Canvas_DoubleClick(object sender, EventArgs e)
        {
            if (currentShape is Polyline polyline)
            {
                ((Polyline)currentShape).CompleteShape();
                drawingHistory.AddShape(currentShape);
                currentShape = null;
                RedrawCanvas();
            }
            else if (currentShape is Polygon polygon)
            {
                ((Polygon)currentShape).CompleteShape();
                drawingHistory.AddShape(currentShape);
                currentShape = null;
                RedrawCanvas();
            }
        }

        private void ClearCanvas()
        {
            using (var g = Graphics.FromImage(drawingBitmap))
            {
                g.Clear(Color.White);
            }
            pictureBoxCanvas.Image = drawingBitmap;
        }

        private void RedrawCanvas()
        {
            using (var g = Graphics.FromImage(drawingBitmap))
            {
                g.Clear(Color.White);
                drawingHistory.DrawAll(g);

                if (currentShape != null)
                {
                    currentShape.Draw(g);
                }
            }
            pictureBoxCanvas.Invalidate();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(drawingBitmap, Point.Empty);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Shape files (*.shp)|*.shp";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, drawingHistory.Serialize());
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Shape files (*.shp)|*.shp";
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    drawingHistory.Deserialize(File.ReadAllText(openDialog.FileName));
                    RedrawCanvas();
                }
            }
        }


    }
}