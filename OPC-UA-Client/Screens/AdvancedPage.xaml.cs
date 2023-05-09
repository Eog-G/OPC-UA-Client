using Opc.UaFx.Client;
using Opc.UaFx.Services;
using Opc.UaFx;
using OPC_UA_Client.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using System.Threading;
using System.Timers;
using System.Security.Cryptography.X509Certificates;

namespace OPC_UA_Client.Screens
{

    public partial class AdvancedPage : UserControl
    {
        private OPCClient opcClient = OPCClient.Instance;
        private ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private DispatcherTimer _timer;
        private MainWindow mainWindow;

        private ObservableCollection<OPCNode> items = new ObservableCollection<OPCNode>();

        public System.Timers.Timer timer = new System.Timers.Timer(1000);

        public AdvancedPage()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;

            DataContext = this;

            OPCTagsListView.ItemsSource = items;

            // Set up the timer to call RefreshListViewAsync() every second
            
            timer.Elapsed += async (sender, e) => await RefreshListViewAsync();
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Stop();

            
        }



        private async Task RefreshListViewAsync()
        {
            if (opcClient.connected)
            {
                var itemsToAdd = new List<OPCNode>();

                await Task.Run(() =>
                {
                    foreach (OPCNode opcNode in opcClient.currentNodes)
                    {
                        var tag = opcClient.ReadTag(opcNode.DisplayName);

                        itemsToAdd.Add(new OPCNode() { DisplayName = opcNode.DisplayName, Value = tag, DataType = opcNode.DataType });
                    }
                });

                Dispatcher.Invoke(() =>
                {
                    OPCTagsListView.BorderBrush = Brushes.Transparent;
                    items.Clear();

                    foreach (var item in itemsToAdd)
                    {
                        items.Add(item);
                    }
                });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Get a reference to the parent window
            mainWindow = Window.GetWindow(this) as MainWindow;

            // Unsubscribe from the loaded event
            Loaded -= UserControl_Loaded;

        }

        
    }
}
