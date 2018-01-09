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
            Tree.mainWindow = this;
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            Tree.SetTree(await Task.Run(() =>
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
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
