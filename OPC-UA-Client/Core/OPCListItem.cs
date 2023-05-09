using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_UA_Client.Core
{
    public class OPCNode
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}
