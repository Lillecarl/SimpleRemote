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


            var TreeEntries = new ObservableCollection<TreeEntry>();
            Tree.ItemsSource = TreeEntries;

            var G1 = new GroupEntry() { Name = "G1" };
            var G11 = new GroupEntry() { Name = "G11" };
            var GM111 = new ConnectionEntry() { Name = "G111" };
            G11.Members.Add(GM111);
            G1.Members.Add(G11);
            TreeEntries.Add(G1);

            var G2 = new GroupEntry() { Name = "G2" };
            var G21 = new GroupEntry() { Name = "G21", IsExpanded = true };
            var GM211 = new ConnectionEntry() { Name = "G211" };
            G21.Members.Add(GM211);
            var GM212 = new ConnectionEntry() { Name = "G212" };
            G21.Members.Add(GM212);
            G2.Members.Add(G21);
            TreeEntries.Add(G2);
        }
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
