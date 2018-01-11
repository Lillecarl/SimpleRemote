using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleShared.Packets
{
    public class SimplePacket
    {
        [JsonProperty]
        public string opcode { get; private set; }
        [JsonProperty]
        public string data { get; private set; }
    }
}
