using System;
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

            Tree.ItemsSource = TreeEntries;
        }

        public void SetTree(ObservableCollection<TreeEntry> Tree)
        {
            TreeEntries.Clear();

            foreach (var i in Tree)
                TreeEntries.Add(i);
        }

        private ObservableCollection<TreeEntry> TreeEntries = new ObservableCollection<TreeEntry>();

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

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var treeentry = Tree.SelectedItem;
                
                if (treeentry != null)
                {
                    DataObject dragData = new DataObject("treeentry", treeentry);
                    DragDrop.DoDragDrop(Tree, dragData, DragDropEffects.Move);
                }
            }
        }

        private void TreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("treeentry") || sender == e.Source)
                e.Effects = DragDropEffects.None;
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            var treeviewitem = sender as TreeViewItem;
            var itemspresenter = e.Source as ItemsPresenter;;

            if (e.Data.GetDataPresent("treeentry"))
            {
                var treeentry = e.Data.GetData("treeentry") as TreeEntry;
                var target = treeviewitem.Header as TreeEntry;

                foreach (var i in TreeEntries)
                    i.RemoveEntry(treeentry);

                target.IsExpanded = true;
                target.Children.Add(treeentry);
            }
        }

        private void MoveEntry(TreeEntry mover, TreeEntry target)
        {
            
        }

        #endregion
    }

    public enum EntryType
    {
        FOLDER,
        RDP,
        WINBOX,
        HTTP,
    }

    public class TreeEntry
    {
        public TreeEntry()
        {
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CountStr = "";
        }

        public int EntryID = 0;

        public string Name { get; set; } = "";
        public string Icon { get; set; } = "";

        public bool IsSelected { get; set; } = false;
        public bool IsExpanded { get; set; } = false;

        public string CountStr
        {
            get
            {
                if (Count > 0)
                    return string.Format(" ({0})", Count);

                return "";
            }
            set
            {

            }
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

        public ObservableCollection<TreeEntry> Children { get; set; } = new ObservableCollection<TreeEntry>();
    }
}
