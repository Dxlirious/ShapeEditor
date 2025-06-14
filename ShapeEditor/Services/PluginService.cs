using System;
using System.Reflection;
using System.Windows.Forms;
using ShapeEditor.Core;

public class PluginService
{
    public void LoadPlugin(string pluginPath, ToolStrip toolStrip, Action<string> shapeTypeSetter)
    {
        try
        {
            var assembly = Assembly.LoadFrom(pluginPath);

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(Shape).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    ShapeFactory.RegisterShape(type.Name, () => (Shape)Activator.CreateInstance(type));

                    var button = new ToolStripButton(type.Name)
                    {
                        DisplayStyle = ToolStripItemDisplayStyle.Text,
                        Tag = pluginPath
                    };
                    button.Click += (s, e) => shapeTypeSetter(type.Name);

                    int insertIndex = toolStrip.Items.Count - 3;
                    toolStrip.Items.Insert(insertIndex, button);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки плагина: {ex.Message}", "Ошибка");
        }
    }
}