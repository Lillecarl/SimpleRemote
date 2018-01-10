using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleRemote.Config
{
    class RDP : IConfigEntry
    {
        public FrameworkElement GetElement()
        {
            var formsHost = new Connections.WinFormHostConnect();
            formsHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            formsHost.VerticalAlignment = VerticalAlignment.Stretch;
            var mstscWrapper = new Connections.MSTSCWrapper();

            formsHost.Child = mstscWrapper;

            return formsHost;
        }
    }
}
