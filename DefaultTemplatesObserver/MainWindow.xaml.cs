using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DefaultTemplatesObserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region -- Constructors --

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion // -- Constructors --

        #region -- Private methods --

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var controlType = typeof(Control);
            var derivedTypes = new List<Type>();

            var assembly = Assembly.GetAssembly(typeof(Control));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(controlType) && !type.IsAbstract && type.IsPublic)
                {
                    derivedTypes.Add(type);
                }
            }

            derivedTypes.Sort((x,y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            LstTypes.ItemsSource = derivedTypes;
        }

        private void LstTypes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var type = (Type) LstTypes.SelectedItem;
                var info = type.GetConstructor(Type.EmptyTypes);
                var control = (Control) info?.Invoke(null);
                control.Visibility = Visibility.Collapsed;
                Grid.Children.Add(control);
                var template = control.Template;

                var settings = new XmlWriterSettings { Indent = true };
                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb, settings);
                XamlWriter.Save(template, writer);

                TxtTemplate.Text = sb.ToString();
                Grid.Children.Remove(control);
            }
            catch (Exception exception)
            {
                TxtTemplate.Text = $"<<Error generation template: {exception.Message}>>";
            }
        }

        #endregion // -- Private methods --
    }
}
