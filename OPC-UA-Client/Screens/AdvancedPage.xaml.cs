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

        private List<string> tagIDs = new List<string>();
        private ObservableCollection<OPCListItem> items = new ObservableCollection<OPCListItem>();

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
                tagIDs = opcClient.AllTagIDs;

                var itemsToAdd = new List<OPCListItem>();

                await Task.Run(() =>
                {
                    foreach (string tagID in tagIDs)
                    {
                        var tag = opcClient.ReadTag(tagID);

                        itemsToAdd.Add(new OPCListItem() { DisplayName = tagID, Value = tag, DataType = "DataType1" });
                    }
                });

                Dispatcher.Invoke(() =>
                {
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

        private void Traverse_Nodes(OpcClient client, bool realTimeOnly = false)
        {
            // Browse the server's address space
            var root = client.BrowseNode(OpcObjectTypes.RootFolder);

            // Recursively traverse the address space and print the TagIDs of all nodes
            TraverseNodes(root, false);

            void TraverseNodes(OpcNodeInfo node, bool realTimeTags)
            {

                if (node.NodeId.ToString() == ("ns=2;s=Realtimedata") && realTimeOnly == true) { realTimeTags = true; }

                // Print the TagID of this node
                if (node.NodeId.Value is string tagId && (realTimeTags || !realTimeOnly))
                {
                    
                    OpcValue readNode = opcClient.ReadTag(tagId);

                    items.Add(new OPCListItem() { DisplayName = node.DisplayName, Value = client.ReadNode(node.NodeId).ToString(), DataType = readNode.DataType.ToString() });

                }

                // Recursively traverse child nodes
                foreach (var child in node.Children())
                {
                    TraverseNodes(child, realTimeTags);
                }
            }
        }
    }
}
