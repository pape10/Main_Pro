using ChattingInterfaces;
using ChattingServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : IClient
    {
        IChattingService Server = new ChattingService();
        public string Decrypt(string input, string key)
        {
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

        public string Encrypt(string input, string key)
        {
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

        public void GetMessage(string message, string userName)
        {
            ((MainWindow)Application.Current.MainWindow).TakeMessage(message,userName); 
        }

        public void GetUpdate(int value,string userName)
        {
            switch(value)
            {
                case 0:
                    {
                        ((MainWindow)Application.Current.MainWindow).AddUserToList(userName);
                        break;
                    }

                case 1:
                    {
                        ((MainWindow)Application.Current.MainWindow).RemoveUserToList(userName);
                        break;
                    }

            }
        }

        public string requestForSPN()
        {
            string path = "c:\\Users\\Public\\KerbTrayTGT.txt";

            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string s = "2017-06-28 01:01:53.523";
                sw.WriteLine(Convert.ToDateTime(s));
                sw.Flush();
                sw.Close();
            }

            StreamReader r = new StreamReader(path);
            string tgt = "";
            while (r.EndOfStream == false)
            {
                tgt= r.ReadLine();
            }
            r.Close();
            //MessageBox.Show(tgt);
            if(tgt=="")
            {
                return "";
            }
            string session =Server.GenerateSesionToken(tgt, "Spn1");
            return session;
        }
    }
}
