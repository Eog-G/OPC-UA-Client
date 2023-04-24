using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using Opc.Ua;
using Opc.UaFx;
using Opc.UaFx.Client;
using Opc.UaFx.Services;

namespace Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            OpcClient client = new OpcClient("opc.tcp://eog-pc:62640/IntegrationObjects/ServerSimulator");
            client.Connect();

            Traverse_Nodes(client, true);

            var nod = client.BrowseNode("2:Tag1");
            System.Console.WriteLine(client.ReadNode(nod.NodeId));

            client.WriteNode("2:Tag14", 23);            
            System.Console.ReadKey();
        
            
        }

        static void Traverse_Nodes(OpcClient client, bool realTimeOnly = false)
        {
            // Browse the server's address space
            var root = client.BrowseNode(OpcObjectTypes.RootFolder);
            
            // Recursively traverse the address space and print the TagIDs of all nodes
            TraverseNodes(root, false);

            void TraverseNodes(OpcNodeInfo node, bool realTimeTags)
            {
                OpcBrowseNode bNode = new OpcBrowseNode(node.NodeId);

                if (node.NodeId.ToString() == ("ns=2;s=Realtimedata") && realTimeOnly == true) { realTimeTags = true; }

                // Print the TagID of this node
                if (node.NodeId.Value is string tagId && (realTimeTags || !realTimeOnly))
                {
                    OpcWriteNode opcWriteNode = new OpcWriteNode(node.NodeId, client);
                    OpcWriteNodesRequest request = new OpcWriteNodesRequest(opcWriteNode);

                    

                    System.Console.WriteLine(realTimeTags);
                    System.Console.WriteLine($"Display name: {node.DisplayName}");
                    System.Console.WriteLine($"NodeID: {node.NodeId}");
                    System.Console.WriteLine($"Value: {client.ReadNode(node.NodeId)}");
                    System.Console.WriteLine($"Category: {node.Category}\nReference: {node.Reference}\nContext: {node.Context}");
                    System.Console.WriteLine($"No idea: {request.GetType()}");
                    System.Console.WriteLine($"\n");
                    
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
