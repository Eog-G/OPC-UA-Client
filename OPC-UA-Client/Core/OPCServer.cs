using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace OPC_UA_Client.Core
{
    public class OPCServer
    {
        public string testValue;

        private static OPCServer instance = null;
        private static readonly object padlock = new object();

        public bool connected;
        public string EndpointURL { get; set; }

        public OpcValue tag99 { get; set; }

        private OpcClient client;

        public OPCServer()
        {
            this.EndpointURL = "";
            this.tag99 = 0;
            

            try
            {
                client = new OpcClient(EndpointURL);
                client.Connect();
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
            }
        }

        public static OPCServer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new OPCServer();
                    }
                    return instance;
                }
            }
        }

        public void WriteValue(string tagID, Int16 value)
        {
            if(connected)
            {
                client.WriteNode(tagID, value);
            }
        }

        public string ReadTag99(int index)
        {
            if(connected)
            {
                tag99 = client.ReadNode("2:Tag99");
            }

            return tag99.ToString();
        }
        
        public void Connect()
        {
            try
            {
                client = new OpcClient(EndpointURL);
                client.Connect();
                connected = true;
            }
            catch (Exception ex)
            {
                client.ServerAddress = new Uri("");
                connected = false;
            }
        }

        public void Disconnect()
        {
            if (connected)
            {
                try
                {
                    client.Disconnect();
                    connected = false;
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
