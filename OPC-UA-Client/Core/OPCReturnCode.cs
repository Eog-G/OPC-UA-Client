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
        public Tuple<int, string> Code;
        public readonly string correctType;
        
        public OPCReturnCode(int code, string type = null)
        {
            if (code == -1)
            {
                Code = new Tuple<int, string>(code, "Unkown Error");
            }
            if (code == 0)
            {
                Code = new Tuple<int, string>(code, "Good");
            }
            if (code == 1)
            {
                Code = new Tuple<int, string>(code, "Incorrect Data Type");
                correctType = type;
            }
            if (code == 2)
            {
                Code = new Tuple<int, string>(code, "OPC Server Not Connected");
                
            }
            if (code == 3)
            {
                Code = new Tuple<int, string>(code, "Invalid Tag ID");
            }
        }
    }
}
