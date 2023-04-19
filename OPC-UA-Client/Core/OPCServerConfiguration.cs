using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace OPC_UA_Client.Core
{
    internal class OPCServerConfiguration
    {
        public string url { get; set; }
        public string[] tagNames { get; set; }

        public OpcValue tagValue { get; set; }

        private readonly OpcClient client;

        public OPCServerConfiguration(string url, string[] tagNames)
        {
            this.url = url;
            this.tagNames = tagNames;
            this.tagValue = 0;
            client = new OpcClient(url);
            client.Connect();

        }
        
        public void WriteValue(Int16 value)
        {
            client.WriteNode(tagNames[0], value);
        }

        public string ReadValue(int index)
        {
            tagValue = client.ReadNode(tagNames[index]);

            return tagValue.ToString();
        }
        
        public void CloseConnection()
        {
            client.Disconnect();
        }
    }
}
