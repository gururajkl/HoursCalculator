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
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var allowedChars = new[] { ":", "A", "M", "P", "M" };

            if (!char.IsDigit(e.Text[0]) && !allowedChars.Contains(e.Text))
            {
                e.Handled = true; // Prevent the input if it's not a number or allowed special character
            }
        }
    }
}
