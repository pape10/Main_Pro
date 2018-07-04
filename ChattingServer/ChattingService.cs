using ChattingInterfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace ChattingServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChattingService : IChattingService
    {
        public ConcurrentDictionary<string, ConnectedClient> _connectedClients = new ConcurrentDictionary<string, ConnectedClient>();
        string Ser_Key = "sblw-3hn8-sqoy19";
        
        


        public void Logout()
        {
            ConnectedClient client = GetMyClient();
            if(client != null)
            {
                ConnectedClient removedClient;

                _connectedClients.TryRemove(client.UserName, out removedClient);
                updateHelper(1, removedClient.UserName);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Client logged off:{0} at {1}",removedClient.UserName,System.DateTime.Now);
                Console.ResetColor();
            }
        }
        public ConnectedClient GetMyClient()
        {
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            foreach(var client in _connectedClients)
            {
                if(client.Value.connection == establishedUserConnection)
                {
                    return client.Value;
                }
            }
            return null;
        }
        public void SendMessageToAll(string message, string userName)
        {
            foreach(var client in _connectedClients)
            {
                if(client.Key.ToLower() != userName.ToLower())
                {
                    client.Value.connection.GetMessage(message,userName);
                }
            }
        }
        private void updateHelper(int value,string userName)
        {
            foreach (var client in _connectedClients)
            {
                if(client.Value.UserName.ToLower() != userName.ToLower())
                client.Value.connection.GetUpdate(value, userName);
            }
        }

        public List<string> GetCurrentUsers()
        {
            List<string> listOfUsers = new List<string>();
            foreach(var client in _connectedClients)
            {
                listOfUsers.Add(client.Value.UserName);
            }
            return listOfUsers;
        }

        public string Encrypt(string input, string key1)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                b.Append(key1[i % key1.Length]);
            }
            key1 = b.ToString();
            byte[] key2 = Encoding.Unicode.GetBytes(key1);
            string key = System.Text.Encoding.Default.GetString(key2);
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public string Decrypt(string input, string key1)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                b.Append(key1[i % key1.Length]);
            }
            key1 = b.ToString();
            byte[] key2 = Encoding.Unicode.GetBytes(key1);
            string key = System.Text.Encoding.Default.GetString(key2);
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string KLogin(string username, string auth)
        {
            string con_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Pradeepta_Ranjan_Cho\Documents\DB\Kerberos_Custom_DB.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection sqlcon = new SqlConnection(con_string);
            sqlcon.Open();
            string query = "select * from Users where Username ='"+username+"'";
            SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);
            string pass = "";
            if (dtbl.Rows.Count == 1)
            {
                pass = dtbl.Rows[0][3].ToString();
                
            }
            string authenticator = "";
            try
            {
                authenticator = Decrypt(auth, pass);
            }
            catch(Exception e)
            {
                return "";
            }
            foreach (var client in _connectedClients)
            {
                if (client.Key.ToLower() == username.ToLower())
                {
                    return "";
                }
            }
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            ConnectedClient newClient = new ConnectedClient();
            newClient.connection = establishedUserConnection;
            newClient.UserName = username;

            _connectedClients.TryAdd(username, newClient);
            updateHelper(0, username);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client logged in:{0} at {1}", newClient.UserName, System.DateTime.Now);
            Console.ResetColor();

            string returnString = "";
            returnString = Encrypt(Convert.ToString(DateTime.Now),Ser_Key);
            sqlcon.Close();
            return returnString;
        }

        public string GenerateSesionToken(string tgt, string spn)
        {
            string ses_key = "";
            if (spn == "Spn1")
            {
                ses_key = "pape-3pa8-sqoy19";
            }
            string token = Encrypt(Convert.ToString(DateTime.Now), ses_key);
            return token;
        }
    }
}
