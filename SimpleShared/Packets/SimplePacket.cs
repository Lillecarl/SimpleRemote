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

        public SimplePacket() { } // This exists only for Json.net

        public SimplePacket(object package)
        {
            opcode = package.GetType().Name;
            data = JsonConvert.SerializeObject(package);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator string (SimplePacket packet)
        {
            return packet.ToString();
        }
    }
}
