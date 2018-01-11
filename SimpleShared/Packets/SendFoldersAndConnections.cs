using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShared.Packets
{
    public class SendFoldersAndConnections
    {
        public List<IConfigEntry> configentries { get; set; } = new List<IConfigEntry>();
    }
}
