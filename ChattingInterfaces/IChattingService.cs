using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChattingInterfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IChattingService" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IClient))]
    public interface IChattingService
    {
        [OperationContract]
        void SendMessageToAll(string message,string userName);
        [OperationContract]
        void Logout();
        [OperationContract]
        List<string> GetCurrentUsers();
        [OperationContract]
        string Encrypt(string input, string key);
        [OperationContract]
        string Decrypt(string input, string key);
        [OperationContract]
        string KLogin(string username,string auth);
        [OperationContract]
        string GenerateSesionToken(string tgt, string spn);
    }

}
