﻿using CefSharp.Wpf;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleRemote
{
    /// <summary>
    /// Interaction logic for ConnectionTree.xaml
    /// </summary>
    public partial class ConnectionTree : UserControl
    {
        public ConnectionTree()
        {
            InitializeComponent();
        }

        public MainWindow mainWindow = null;

        public void SetTree(TreeEntry Tree)
        {
            RootEntry.Children.Clear();

            foreach (var i in Tree.Children)
                RootEntry.Children.Add(i);
        }

        public TreeEntry RootEntry { get; set; } = new TreeEntry();

        #region Dragndrop

        Point startPoint = new Point();

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            startPoint = e.GetPosition(null);
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            // Allow some mouse movement before starting the dragging
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var treeentry = Tree.SelectedItem;
                
                if (treeentry != null)
                {
                    DataObject dragData = new DataObject(treeentry.GetType(), treeentry);
                    DragDrop.DoDragDrop(Tree, dragData, DragDropEffects.Move);
                }
            }
        }

        private void TreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeEntry)) || sender == e.Source)
                e.Effects = DragDropEffects.None;
            else if (e.OriginalSource is TextBlock)
                (e.OriginalSource as TextBlock).FontWeight = FontWeights.Bold;
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
                (e.OriginalSource as TextBlock).FontWeight = FontWeights.Normal;
        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
                (e.OriginalSource as TextBlock).FontWeight = FontWeights.Normal;

            var treeviewitem = sender as TreeViewItem;
            var itemspresenter = e.Source as ItemsPresenter;

            if (e.Data.GetDataPresent(typeof(TreeEntry)) && !e.Handled)
            {
                var mover = e.Data.GetData(typeof(TreeEntry)) as TreeEntry;

                var target = treeviewitem.Header as TreeEntry;

                MoveEntry(mover, target, e);
            }
        }

        private void MoveEntry(TreeEntry mover, TreeEntry target, DragEventArgs e)
        {
            if (mover.FindEntry(target) || mover == target)
            {
                e.Handled = true;
                return;
            }

            var mover_parent = RootEntry.GetParent(mover);
            var target_parent = RootEntry.GetParent(target);

            var mousePos = e.GetPosition(e.OriginalSource as FrameworkElement);
            var elementHeight = (e.OriginalSource as FrameworkElement).ActualHeight;
            // Reordering
            if (mover_parent == target_parent)
            {
                bool before = mousePos.Y <= elementHeight / 3;
                bool after = mousePos.Y >= elementHeight - elementHeight / 3;

                if (before || after)
                {
                    mover_parent.Children.Remove(mover);
                    mover_parent.Children.Insert(mover_parent.Children.IndexOf(target) + (before ? 0 : 1), mover);

                    e.Handled = true;
                    return;
                }
            }

            // If the target is the parent of the mover, move to the parent of the target instead
            if (target == mover_parent)
                target = RootEntry.GetParent(target);

            target.IsExpanded = true;
            e.Handled = true;

            RootEntry.RemoveEntry(mover);

            target.Children.Add(mover);
        }

        #endregion

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

                        mainWindow.ConnectionTabs.Items.Add(tab);
                        mainWindow.ConnectionTabs.SelectedItem = tab;
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

    public enum EntryType
    {
        FOLDER,
        RDP,
        WINBOX,
        HTTP,
    }

    public class TreeEntry : INotifyPropertyChanged
    {
        public TreeEntry()
        {
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CountStr = "";
        }

        public int entryID = 0;
        public EntryType entryType = EntryType.FOLDER;
        public SimpleRemote.Config.IConfigEntry config = null;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";

        public bool IsSelected { get; set; } = false;

        private bool _IsExpanded = false;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        public string CountStr
        {
            get { return Count > 0 ? string.Format(" ({0})", Count) : ""; }
            set { OnPropertyChanged("CountStr"); }
        }

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; OnPropertyChanged("Visibility"); }
        }

        private int Count
        {
            get
            {
                int count = Children.Count;

                foreach (var child in Children)
                    count += child.Count;

                return count;
            }
        }

        public void RemoveEntry(TreeEntry entry)
        {
            Children.Remove(entry);

            foreach (var child in Children)
                child.RemoveEntry(entry);
        }

        public bool FindEntry(TreeEntry entry)
        {
            if (Children.Contains(entry))
                return true;

            foreach (var child in Children)
                if (child.FindEntry(entry))
                    return true;

            return false;
        }

        public TreeEntry GetParent(TreeEntry entry)
        {
            if (Children.Contains(entry))
                return this;

            foreach (var child in Children)
                if (child.GetParent(entry) != null)
                    return child.GetParent(entry);

            return null;
        }

        public ObservableCollection<TreeEntry> Children { get; set; } = new ObservableCollection<TreeEntry>();
    }
}
