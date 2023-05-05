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

                if(nodeType == "UInt16") // I know this is a bit silly but for some reason node type won't match C# types
                {
                    if (UInt16.TryParse(value, out UInt16 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "UInt16"); } // Incorrect Data Type
                }
                if (nodeType == "Int16")
                {
                    if (Int16.TryParse(value, out Int16 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Int16"); } // Incorrect Data Type
                }
                if (nodeType == "UInt32")
                {
                    if (UInt32.TryParse(value, out UInt32 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "UInt32"); } // Incorrect Data Type
                }
                if (nodeType == "Int32")
                {
                    if (Int32.TryParse(value, out Int32 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Int32"); } // Incorrect Data Type
                }
                if (nodeType == "UInt64")
                {
                    if (UInt64.TryParse(value, out UInt64 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "UInt64"); } // Incorrect Data Type
                }
                if (nodeType == "Int64")
                {
                    if (Int64.TryParse(value, out Int64 res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Int64"); } // Incorrect Data Type
                }
                if (nodeType == "Double")
                {
                    if (Double.TryParse(value, out Double res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Double"); } // Incorrect Data Type
                }
                if (nodeType == "Byte")
                {
                    if (Byte.TryParse(value, out Byte res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Byte"); } // Incorrect Data Type
                }
                if (nodeType == "Boolean")
                {
                    if (Boolean.TryParse(value, out Boolean res))
                    {
                        client.WriteNode(RWTag, res);
                        return new OPCReturnCode(0); // OK
                    }
                    else { return new OPCReturnCode(1, "Boolean"); } // Incorrect Data Type
                }
                if (nodeType == "String")
                {
                    client.WriteNode(RWTag, value);
                    return new OPCReturnCode(0); // OK    
                }
                else { return new OPCReturnCode(-1); }
                    
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
