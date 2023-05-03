using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_UA_Client.Core
{
    public class OPCReturnCode
    {
        public Tuple<string, int> Code;
        
        public OPCReturnCode(int code)
        {
            if (code == -1)
            {
                Code = new Tuple<string, int>("Unkown Error", code);
            }
            if (code == 0)
            {
                Code = new Tuple<string, int>("Good", code);
            }
            if (code == 1)
            {
                Code = new Tuple<string, int>("Incorrect Data Type", code);
            }
            if (code == 2)
            {
                Code = new Tuple<string, int>("OPC Server Not Connected", code);
                
            }
        }
    }
}
