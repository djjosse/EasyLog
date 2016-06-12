using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace EasyLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogWindow : Window, IDisposable
    {
        eCategory _logCategoryFilter = eCategory.Debug;
        string _logModuleFilter = Log.ALL;
        private bool? _isAutoscroll = true;
        MessageListener _listener = new MessageListener();
        List<string> _moduleTags = new List<string>();
        bool _windowDisposed = false;
        public LogWindow(string title, List<string> moduleTags)
        {
            InitializeComponent();
            Closing += Window_Closing;
            this.Title = title;
            _moduleTags.Add(Log.ALL);
            _moduleTags.AddRange(moduleTags);
            Initialize();
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Initialize()
        {
            //Set Log Listener
            Log.AttachListener(_listener);

            //Set Item source for list view of UI log and set syncronization options
            LogMessageList.ItemsSource = _listener.AllMessages;
            BindingOperations.EnableCollectionSynchronization(_listener.AllMessages, _listener.Token);

            //define filtering method
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(LogMessageList.ItemsSource);
            view.Filter = CheckLogFilter;

            //set on UI log on change event
            _listener.AllMessages.CollectionChanged += LogViewChanged;

            //init filter combo boxes
            LevelFilter.ItemsSource = Enum.GetValues(typeof(eCategory)).Cast<eCategory>();
            LevelFilter.Text = eCategory.Debug.ToString();
            ModuleFilter.ItemsSource = _moduleTags;
            ModuleFilter.Text = Log.ALL;
        }

        private void LogViewChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (_isAutoscroll ?? false)
                {
                    int count = LogMessageList.Items.Count;
                    if (count > 0)
                    {
                        var last = LogMessageList.Items[count - 1];
                        LogMessageList.ScrollIntoView(last);
                    }
                }
            }));
        }

        private bool CheckLogFilter(object item)
        {
            int currentCategory = (int)(item as Message).Category;
            int selectedCategory = (int)_logCategoryFilter;

            int selectedModule = _moduleTags.IndexOf(_logModuleFilter);
            bool isInModule = (selectedModule == 0)
                || _moduleTags.IndexOf((item as Message).Module) == selectedModule;

            return currentCategory >= selectedCategory && isInModule;
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = ((ComboBox)sender).SelectedItem.ToString();
            if (((ComboBox)sender).Name.Equals("LevelFilter"))
            {
                Enum.TryParse(selection, out _logCategoryFilter);
            }
            else if (((ComboBox)sender).Name.Equals("ModuleFilter"))
            {
                _logModuleFilter = selection;
            }
            CollectionViewSource.GetDefaultView(LogMessageList.ItemsSource).Refresh();
        }

        private void AutoscrollCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            _isAutoscroll = (sender as CheckBox).IsChecked;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
            {
                Hide();
                return null;
            }, null);

            //Close Only if Dispose Log was called
            e.Cancel = !_windowDisposed;
        }

        /// <summary>
        /// Disposing Log Destroys the window
        /// </summary>
        public void Dispose()
        {
            _windowDisposed = true;
            this.Close();
        }
    }
}
