using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HoursCalculator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChangeTheme();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var allowedChars = new[] { ":", "A", "M", "P", "M" };

            if (!char.IsDigit(e.Text[0]) && !allowedChars.Contains(e.Text))
            {
                e.Handled = true; // Prevent the input if it's not a number or allowed special character
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void ChangeTheme()
        {
            string theme = "/Resources/Light.xaml";

            if (HoursCalculator.Properties.Settings.Default.DarkMode)
                theme = "/Resources/Dark.xaml";
            else
                theme = "/Resources/Light.xaml";

            var resources = Application.Current.Resources;
            resources.MergedDictionaries.RemoveAt(1);
            resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri(theme, UriKind.Relative) });
        }
    }
}
