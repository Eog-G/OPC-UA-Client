using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using static Opc.UaFx.OpcObjectTypes;

namespace OPC_UA_Client.Core
{
    public class OPCClient
    {
        public List<OPCNode> currentNodes = new List<OPCNode>();

        public string RWTag; // Tag specifically chosen to Read / Write to on main page

        public string RWTagType // Gets RWTag data type
        { 
            get 
            {
                try
                {
                    OpcValue node = client.ReadNode(RWTag);
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

        private static OPCClient instance = null;
        private static readonly object padlock = new object();

        public bool connected;
        public string EndpointURL { get; set; }

        private OpcClient client;

        public OPCClient()
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

        public static OPCClient Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new OPCClient();
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

        public void PopulateCurrentOPCNodes(bool realTimeOnly = true)
        {
            if (connected)
            {
                currentNodes.Clear();

                // Browse the server's address space
                var root = client.BrowseNode(OpcObjectTypes.RootFolder);

                // Recursively traverse the address space and print the TagIDs of all nodes
                TraverseNodes(root, false);

                void TraverseNodes(OpcNodeInfo node, bool realTimeTags)
                {
                    if (connected)
                    {
                        if (node.NodeId.ToString() == ("ns=2;s=Realtimedata") && realTimeOnly == true) { realTimeTags = true; }

                        // Print the TagID of this node
                        if (node.NodeId.Value is string tagId && (realTimeTags || !realTimeOnly) && !tagId.Contains("Realtimedata"))
                        {
                            OPCNode opcNode = new OPCNode();
                            OpcValue readNode = client.ReadNode("2:" + tagId);

                            opcNode.DisplayName = "2:" + node.DisplayName;
                            //opcNode.Value = readNode.Value.ToString(); Value is not needed as it is read each time within the advanced page
                            opcNode.DataType = readNode.DataType.ToString();


                            currentNodes.Add(opcNode);
                        }

                        // Recursively traverse child nodes
                        foreach (var child in node.Children())
                        {
                            TraverseNodes(child, realTimeTags);
                        }
                    }
                }
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
