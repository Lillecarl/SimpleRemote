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

using SimpleRemote.ViewModels;

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
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            SetTree(await Task.Run(() =>
            {
                var RootEntry = new TreeEntry();
                var G1 = new TreeEntry() { Name = "G1" };
                var G11 = new TreeEntry() { Name = "G11" };
                var GM111 = new TreeEntry() { Name = "G111" };
                G11.Children.Add(GM111);
                G1.Children.Add(G11);
                RootEntry.Children.Add(G1);

                var G2 = new TreeEntry() { Name = "G2", IsExpanded = true };
                var G21 = new TreeEntry() { Name = "G21", IsExpanded = true };
                var GM211 = new TreeEntry() { Name = "G211" };
                G21.Children.Add(GM211);
                var GM212 = new TreeEntry() { Name = "G212 (RDP)" };
                GM212.config = new Config.RDP();
                G21.Children.Add(GM212);
                var GM213 = new TreeEntry() { Name = "G213 (WWW)" };
                GM213.config = new Config.WWW();
                G21.Children.Add(GM213);
                G2.Children.Add(G21);
                RootEntry.Children.Add(G2);

                return RootEntry;
            }));
        }

        public void SetTree(TreeEntry Tree)
        {
            RootEntry.Children.Clear();

            foreach (var i in Tree.Children)
                RootEntry.Children.Add(i);
        }

        public TreeEntry RootEntry { get; set; } = new TreeEntry();

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                var treeViewItem = sender as TreeViewItem;
                var treeEntry = treeViewItem.Header as TreeEntry;

                if (treeEntry.config != null)
                {
                    var control = treeEntry.config.GetElement();

                    if (control != null)
                    {
                        var tab = new TabItem();
                        tab.Header = treeEntry.Name;
                        tab.Content = control;
                        tab.Loaded += Tab_Loaded;

                        tab.DataContext = tab;
                        tab.ContextMenu = new ContextMenu();
                        var closebtn = new MenuItem();
                        closebtn.Header = "Close";
                        closebtn.Click += Closebtn_Click;
                        tab.ContextMenu.Items.Add(closebtn);

                        ConnectionTabs.Items.Add(tab);
                        ConnectionTabs.SelectedItem = tab;
                    }
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

        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TabItem))
                return;

            var tabitem = sender as TabItem;

            if (tabitem.Content is Connections.IConnection)
                (tabitem.Content as Connections.IConnection).Connect();
        }
    }
}
