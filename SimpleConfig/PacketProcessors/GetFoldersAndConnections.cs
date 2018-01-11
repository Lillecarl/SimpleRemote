using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleShared.Config;
using SimpleShared.Packets;

namespace SimpleConfig.PacketProcessors
{
    class GetFoldersAndConnections : IPacketProcessor
    {
        public string GetName()
        {
            return GetType().Name;
        }

        public object ProcessPacket(string packet)
        {
            var foldersAndConnections = new SendFoldersAndConnections();

            var config1 = new RDP();
            config1.EntryID = 0;
            config1.ParentID = 0;
            config1.Name = "FirstEntry";
            foldersAndConnections.configentries.Add(config1);

            return foldersAndConnections;
        }
    }
}
