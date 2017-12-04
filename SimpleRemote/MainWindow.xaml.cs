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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                var TreeEntries = new ObservableCollection<TreeEntry>();

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

                e.Result = TreeEntries;
            };

            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                var result = e.Result as ObservableCollection<TreeEntry>;

                Tree.SetTree(result);
            };

            worker.RunWorkerAsync();
        }
    }
}
