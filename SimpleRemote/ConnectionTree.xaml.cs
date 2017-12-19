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

        Point _lastMouseDown;
        TreeEntry draggedItem, _target;


        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(Tree);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (TreeEntry)Tree.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Tree, Tree.SelectedValue, DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
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

        public ObservableCollection<TreeEntry> Children { get; set; } = new ObservableCollection<TreeEntry>();
    }
}
