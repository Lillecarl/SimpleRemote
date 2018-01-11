using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleConfig.MessageProcessors
{
    class GetFoldersAndConnections : IPacketProcessor
    {
        public string GetName()
        {
            return GetType().Name;
        }

        public object ProcessPacket(string packet)
        {
            return "";
        }
    }
}
