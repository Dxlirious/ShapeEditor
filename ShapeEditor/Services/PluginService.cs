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

            var pluginAssembly = Assembly.LoadFrom(pluginPath);

            bool pluginLoaded = false;

            foreach (var type in pluginAssembly.GetTypes())
            {
                if (typeof(Shape).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    try
                    {

                        ShapeFactory.RegisterShape(type.Name, () =>
                        {
                            try
                            {
                                return (Shape)Activator.CreateInstance(type);
                            }
                            catch
                            {
                                return null;
                            }
                        });

                        var button = new ToolStripButton(type.Name)
                        {
                            DisplayStyle = ToolStripItemDisplayStyle.Text,
                            Tag = pluginPath
                        };

                        button.Click += (s, e) => shapeTypeSetter(type.Name);


                        int insertIndex = toolStrip.Items.Count - 3;
                        if (insertIndex < 0) insertIndex = 0;

                        toolStrip.Items.Insert(insertIndex, button);

                        pluginLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка регистрации типа {type.Name}: {ex.Message}");
                    }
                }
            }

            if (!pluginLoaded)
            {
                MessageBox.Show("Не найдено ни одного корректного класса фигуры в плагине");
            }
        }
        catch (BadImageFormatException)
        {
            MessageBox.Show("Неверный формат DLL. Убедитесь, что плагин собран для правильной версии .NET");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Критическая ошибка загрузки плагина: {ex.Message}");
        }
    }
}