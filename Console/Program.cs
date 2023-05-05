using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
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
            
            OpcClient client = new OpcClient("opc.tcp://uksgclap055:62640/IntegrationObjects/ServerSimulator");
            client.Connect();
            /*
            double dpp = Convert.ToDouble(client.ReadNode("2:DPP").Value);
            double dpr = Convert.ToDouble(client.ReadNode("2:DPR").Value);
            double dppl = Convert.ToDouble(client.ReadNode("2:DPPL").Value);
            double temp = Convert.ToDouble(client.ReadNode("2:Temp").Value);
            double pres = Convert.ToDouble(client.ReadNode("2:Pres").Value);
            double dens = Convert.ToDouble(client.ReadNode("2:Dens").Value);

            
            while (true)
            {
                try
                {
                    // calculate the range for the fluctuation
                    int dppR = (int)(dpp * 0.1);
                    int dprR = (int)(dpr * 0.1);
                    int dpplR = (int)(dppl * 0.1);
                    int tempR = (int)(temp * 0.1);
                    int presR = (int)(pres * 0.1);
                    int densR = (int)(dens * 0.1);

                    // generate a random number within the range
                    int dppF = new Random().Next(-dppR, dppR + 1);
                    int dprF = new Random().Next(-dprR, dprR + 1);
                    int dpplF = new Random().Next(-dpplR, dpplR + 1);
                    int tempF = new Random().Next(-tempR, tempR + 1);
                    int presF = new Random().Next(-presR, presR + 1);
                    int densF = new Random().Next(-densR, densR + 1);

                    // apply the fluctuation to the number
                    var newDpp = dpp + dppF;
                    var newDpr = dpr + dprF;
                    var newDppl = dppl + dpplF;
                    var newTemp = temp + tempF;
                    var newPres = pres + presF;
                    var newDens = dens + densF;

                    // write data back to OPC server
                    client.WriteNode("2:DPP", newDpp);
                    client.WriteNode("2:DPR", newDpr);
                    client.WriteNode("2:DPPL", newDppl);
                    client.WriteNode("2:Temp", newTemp);
                    client.WriteNode("2:Pres", newPres);
                    client.WriteNode("2:Dens", newDens);

                    // write to console
                    System.Console.WriteLine("Updating DPP: " + newDpp);
                    System.Console.WriteLine("Updating DPR: " + newDpr);
                    System.Console.WriteLine("Updating DPPL: " + newDppl);
                    System.Console.WriteLine("Updating Temp: " + newTemp);
                    System.Console.WriteLine("Updating Pres: " + newPres);
                    System.Console.WriteLine("Updating Dens: " + newDens + "\n");

                    // wait for a short time before repeating the loop
                    System.Threading.Thread.Sleep(3000);
                }
                catch
                {
                    break;
                }
            }
            
            */
            
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
