using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using WebSocket4Net;

using SimpleShared.Config;

using SimpleRemote.ViewModels;
using System.Diagnostics;

namespace SimpleRemote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tabs.CollectionChanged += Tabs_SelectNewTab;
        }

        public TreeEntry RootEntry { get; set; } = new TreeEntry();
        public ObservableCollection<TabEntry> Tabs { get; set; } = new ObservableCollection<TabEntry>();
        private WebSocket websocket = null;

        private async void Window_Initialized(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"SimpleConfig.exe");

            websocket = new WebSocket("ws://127.0.0.1:2012/");
            websocket.EnableAutoSendPing = true;
            websocket.Opened += Websocket_Opened;
            websocket.MessageReceived += Websocket_MessageReceived;
            websocket.Open();

            SetTree(await Task.Run(() =>
            {
                var RootEntry = new TreeEntry();
                var G1 = new TreeEntry();
                var C1 = new RDP();
                C1.EntryID = 1;
                C1.ParentID = 0;
                C1.Name = "G1";
                G1.Config = C1;
                RootEntry.Children.Add(G1);

                return RootEntry;
            }));
        }

        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var G1 = new TreeEntry();
            var C1 = new RDP();
            C1.EntryID = 1;
            C1.ParentID = 0;
            C1.Name = "G1";
            G1.Config = C1;
            RootEntry.Children.Add(G1);
        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            var websocket = sender as WebSocket;
            var data = new SimpleShared.Packets.GetFoldersAndConnections();
            var packet = new SimpleShared.Packets.SimplePacket(data);
            websocket.Send(packet);
        }

        private void Tabs_SelectNewTab(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                ConnectionTabs.SelectedIndex = e.NewStartingIndex;
        }

        public void SetTree(TreeEntry Tree)
        {
            RootEntry.Children.Clear();

            foreach (var i in Tree.Children)
                RootEntry.Children.Add(i);
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                var treeViewItem = sender as TreeViewItem;
                var treeEntry = treeViewItem.Header as TreeEntry;

                if (treeEntry.Config != null)
                {
                    //var control = treeEntry.config.GetElement();
                    //
                    //if (control != null)
                    //{
                    //    var tabEntry = new TabEntry();
                    //    tabEntry.Header = treeEntry.Name;
                    //    tabEntry.Content = control;
                    //    Tabs.Add(tabEntry);
                    //
                    //    if (tabEntry.Content is Connections.IConnection)
                    //        (tabEntry.Content as Connections.IConnection).Connect();
                    //}
                }
            }
        }

        private void Closebtn_Click(object sender, RoutedEventArgs e)
        {
            var menuitem = e.Source as MenuItem;

            if (menuitem.DataContext is TabItem)
            {
                var tab = menuitem.DataContext as TabItem;

                if (tab.Parent is TabControl)
                {
                    var tabcontrol = tab.Parent as TabControl;

                    if (tab.Content is ChromiumWebBrowser)
                        (tab.Content as ChromiumWebBrowser).Dispose();

                    tabcontrol.Items.Remove(tab);
                }
            }
        }
    }
}
