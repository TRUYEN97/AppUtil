using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUtil.Communicate.Interface
{
    internal interface IConnectable
    {
        bool Connect();
        bool IsConnect();
        bool Disconnect();
    }
}
