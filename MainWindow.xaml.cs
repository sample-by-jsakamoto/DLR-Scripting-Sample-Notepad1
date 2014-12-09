using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Scripting.Hosting;

namespace Notepad1
{
    public partial class MainWindow : Window
    {

        public ICommand MacroCommand = new RoutedCommand("Macro", typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(this.MacroCommand, Macro_Executed));
        }

        public void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        public void Macro_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var macroPath = e.Parameter as string;

            var scriptRuntime = ScriptRuntime.CreateFromConfiguration();
            var engine = scriptRuntime.GetEngineByFileExtension(Path.GetExtension(macroPath));

            var scope = scriptRuntime.CreateScope();
            scope.SetVariable("textBox", this.textBox1);

            engine.ExecuteFile(macroPath, scope);
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var parentMenu = sender as MenuItem;
            parentMenu.Items.Clear();

            var macrosDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Macros");
            Directory.GetFiles(macrosDir)
                .OrderBy(macroPath => macroPath)
                .Select((macroPath, index) => new MenuItem
                {
                    Header = string.Format("{0}(_{1})", Path.GetFileNameWithoutExtension(macroPath), index + 1),
                    Command = this.MacroCommand,
                    CommandParameter = macroPath
                })
                .ToList()
                .ForEach(menuItem => parentMenu.Items.Add(menuItem));
        }

        private void MenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            var parentMenu = sender as MenuItem;
            parentMenu.Items.Add(new MenuItem { Header = "dummy" });
        }
    }
}
