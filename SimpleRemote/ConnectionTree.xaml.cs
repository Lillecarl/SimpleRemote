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

        public void SetTree(ObservableCollection<TreeEntry> Tree) { TreeEntries = Tree; }

        private ObservableCollection<TreeEntry> TreeEntries = new ObservableCollection<TreeEntry>();
    }


    public class TreeEntry
    {
        public string Name { get; set; } = "";

        public bool IsSelected { get; set; } = false;
        public bool IsExpanded { get; set; } = false;
    }

    public class GroupEntry : TreeEntry
    {
        public ObservableCollection<TreeEntry> Members { get; set; } = new ObservableCollection<TreeEntry>();
        public string ChildrenCount
        {
            get
            {
                return GetChildrenCount().ToString();
            }
        }

        private int GetChildrenCount()
        {
            int count = 0;

            foreach (var child in Members)
                count += child is GroupEntry ? (child as GroupEntry).GetChildrenCount() : 1;

            return count;
        }
    }

    public class ConnectionEntry : TreeEntry
    {
    }
}
