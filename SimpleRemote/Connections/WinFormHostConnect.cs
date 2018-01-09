using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Connections
{
    class WinFormHostConnect : System.Windows.Forms.Integration.WindowsFormsHost, IConnection
    {
        public void Connect()
        {
            if (Child is IConnection)
                (Child as IConnection).Connect();
        }
    }
}
