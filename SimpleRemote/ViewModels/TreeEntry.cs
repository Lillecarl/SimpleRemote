using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using SimpleShared;

namespace SimpleRemote.ViewModels
{
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

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var i in e.NewItems)
                {
                    if (i is TreeEntry)
                    {
                        try
                        {
                            (i as TreeEntry).Config.ParentID = Config.EntryID;
                        }
                        catch { }
                    }
                }
            }
        }

        public IConfigEntry Config { get; set; } = null;

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

        public ObservableCollection<TreeEntry> Children { get; set; } = new ObservableCollection<TreeEntry>();
    }
}
