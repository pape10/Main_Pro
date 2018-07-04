using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ChattingInterfaces
{
    public interface IClient
    {
        [OperationContract]
        void GetMessage(string Message,string userName);
        [OperationContract]
        void GetUpdate(int value,string userName);
        [OperationContract]
        string requestForSPN();
    }
}
