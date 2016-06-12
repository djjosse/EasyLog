using EasyLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> _tags = new List<string>()
        {
            "ModuleA",
            "ModuleB",
            "ModuleC"
        };

        LogWindow logWin = new LogWindow("Example Log", _tags);

        public MainWindow()
        {
            InitializeComponent();

            _level.ItemsSource = Enum.GetValues(typeof(eCategory)).Cast<eCategory>();
            _level.Text = eCategory.Debug.ToString();
            _tag.ItemsSource = _tags;
            _tag.Text = _tags[0];

            Closed += Window_Closing;
            Log.Print("Started", eCategory.Info);
        }

        private void OnOpenLog(object sender, RoutedEventArgs e)
        {
            logWin.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnAddMessage(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(_text.Text) && !String.IsNullOrWhiteSpace(_text.Text))
            {
                string text = _text.Text;
                eCategory category = (eCategory)Enum.Parse(typeof(eCategory), _level.SelectedValue.ToString());
                string tag = _tag.SelectedValue.ToString();
                Log.Print(text, category, tag);
                _text.Text = String.Empty;
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            logWin.Dispose();
        }
    }
}
