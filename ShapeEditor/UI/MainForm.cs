using System;
using System.Drawing;
using System.Windows.Forms;
using ShapeEditor.UI;

namespace ShapeEditor
{
    public partial class MainForm : Form
    {
        private readonly DrawingCanvas _drawingCanvas;

        public MainForm()
        {
            InitializeComponent();
            _drawingCanvas = new DrawingCanvas();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Main menu
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var openItem = new ToolStripMenuItem("Открыть");
            var saveItem = new ToolStripMenuItem("Сохранить");
            var pluginItem = new ToolStripMenuItem("Загрузить плагин");
            var exitItem = new ToolStripMenuItem("Выход");

            openItem.Click += (s, e) => OpenFile();
            saveItem.Click += (s, e) => SaveFile();
            pluginItem.Click += (s, e) => LoadPlugin();
            exitItem.Click += (s, e) => Close();

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openItem, saveItem, pluginItem, exitItem });
            menuStrip.Items.Add(fileMenu);
            Controls.Add(menuStrip);

            // Toolstrip with shape buttons
            var toolStrip = new ToolStrip { Dock = DockStyle.Top };

            // Shape buttons
            var shapes = new[] { "Line", "Rectangle", "Ellipse", "Polyline", "Polygon" };
            foreach (var shape in shapes)
            {
                var button = new ToolStripButton(shape);
                button.Click += (s, e) =>
                {
                    _drawingCanvas.SetShapeType(shape);
                    UpdateButtonStates(toolStrip, button);
                };
                toolStrip.Items.Add(button);
            }

            // Color pickers
            AddColorButton(toolStrip, "Цвет линии", color => _drawingCanvas.SetStrokeColor(color));
            AddColorButton(toolStrip, "Цвет заливки", color => _drawingCanvas.SetFillColor(color));

            // Thickness selector
            AddThicknessControl(toolStrip);

            // Undo/Redo buttons
            AddActionButton(toolStrip, "Отменить", () => _drawingCanvas.Undo());
            AddActionButton(toolStrip, "Повторить", () => _drawingCanvas.Redo());

            Controls.Add(toolStrip);
            Controls.Add(_drawingCanvas);
        }

        private void AddColorButton(ToolStrip toolStrip, string text, Action<Color> colorSetter)
        {
            var button = new ToolStripButton(text);
            button.Click += (s, e) =>
            {
                using (var dialog = new ColorDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        colorSetter(dialog.Color);
                    }
                }
            };
            toolStrip.Items.Add(button);
        }

        private void AddThicknessControl(ToolStrip toolStrip)
        {
            var thicknessCombo = new ToolStripComboBox();
            for (int i = 1; i <= 10; i++) thicknessCombo.Items.Add(i);
            thicknessCombo.SelectedIndex = 1;
            thicknessCombo.SelectedIndexChanged += (s, e) =>
            {
                _drawingCanvas.SetStrokeThickness((int)thicknessCombo.SelectedItem);
            };
            toolStrip.Items.Add(new ToolStripLabel("Толщина:"));
            toolStrip.Items.Add(thicknessCombo);
        }

        private void AddActionButton(ToolStrip toolStrip, string text, Action action)
        {
            var button = new ToolStripButton(text);
            button.Click += (s, e) => action();
            toolStrip.Items.Add(button);
        }

        private void UpdateButtonStates(ToolStrip toolStrip, ToolStripButton activeButton)
        {
            foreach (var item in toolStrip.Items)
            {
                if (item is ToolStripButton button)
                {
                    button.Checked = (button == activeButton);
                }
            }
        }

        private void OpenFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Shape Files|*.shp|All Files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _drawingCanvas.LoadDrawing(dialog.FileName);
                }
            }
        }

        private void SaveFile()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Shape Files|*.shp|All Files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _drawingCanvas.SaveDrawing(dialog.FileName);
                }
            }
        }

        private void LoadPlugin()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Plugin DLLs|*.dll|All Files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _drawingCanvas.LoadPlugin(dialog.FileName, GetToolStrip());
                }
            }
        }

        private ToolStrip GetToolStrip()
        {
            foreach (Control control in Controls)
            {
                if (control is ToolStrip toolStrip)
                {
                    return toolStrip;
                }
            }
            return null;
        }
    }
}