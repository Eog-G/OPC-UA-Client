using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
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

        public string RWTag;

        public string RWTagType 
        { 
            get 
            {
                try
                {
                    var node = client.ReadNode(RWTag);
                    if (node.Value != null)
                    {
                        return node.DataType.ToString();
                    }
                    else
                    {
                        return "null";
                    }
                }
                catch
                {
                    return "null";
                }
            } 
        }

        private static OPCServer instance = null;
        private static readonly object padlock = new object();

        public bool connected;
        public string EndpointURL { get; set; }

        private OpcClient client;

        public OPCServer()
        {
            this.EndpointURL = "";

            try
            {
                client = new OpcClient(EndpointURL);
                client.Connect();
                connected = true;
            }
            catch
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

        public OPCReturnCode WriteValue(string value)
        {
            if(connected)
            {
                var node = client.ReadNode(RWTag);
                var nodeType = node.DataType.ToString();
                

                if(nodeType == "UInt16")
                {

                    if (UInt16.TryParse(value, out UInt16 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else
                    {
                        return new OPCReturnCode(1); // Incorrect Data Type
                    }
                }
                else
                {
                    return new OPCReturnCode(-1);
                }
                
            }
            else
            {
                return new OPCReturnCode(2); // No connection
            }
        }

        public string ReadTag(string tagID)
        {
            if(connected)
            {
                try
                {
                    return client.ReadNode(tagID).ToString();
                }
                catch { return null; }
            }
            return null;
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
