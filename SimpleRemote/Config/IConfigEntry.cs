using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Config
{
    public interface IConfigEntry
    {
        System.Windows.FrameworkElement GetElement();
    }
}
