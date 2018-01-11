using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShared.Packets
{
    public interface IPacketProcessor
    {
        string GetName();
        object ProcessPacket(string packet);
    }
}
