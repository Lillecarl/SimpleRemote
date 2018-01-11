using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SimpleShared.Packets;


namespace SimpleShared.PacketHandling
{
    [Serializable]
    public class DelegatorException : Exception
    {
        public DelegatorException()
        { }

        public DelegatorException(string message)
            : base(message)
        { }

        public DelegatorException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class PacketDelegator
    {
        public string DelegateMessage(string message)
        {
            object replyobject = new object();
            var basemessage = JsonConvert.DeserializeObject<SimplePacket>(message);

            try
            {
                replyobject = processors[basemessage.opcode].ProcessPacket(basemessage.data);
            }
            catch
            {
                throw new DelegatorException("Unable to find message processor");
            }

            string reply = JsonConvert.SerializeObject(replyobject);

            return reply;
        }

        public static void Init()
        {
            var instances = from t in Assembly.GetEntryAssembly().GetTypes()
                              where t.GetInterfaces().Contains(typeof(IPacketProcessor)) && t.GetConstructor(Type.EmptyTypes) != null
                              select Activator.CreateInstance(t) as IPacketProcessor;

            foreach (var i in instances)
                processors.Add(i.GetName(), i);
        }

        private static Dictionary<string, IPacketProcessor> processors = new Dictionary<string, IPacketProcessor>();
    }
}
