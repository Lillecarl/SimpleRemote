using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleRemote.ViewModels
{
    public class TabEntry
    {
        public string Header { get; set; }
        public string Name { get { return Header; } set { Header = value; } } // Fix bind warning
        public FrameworkElement Content { get; set; }
    }
}
