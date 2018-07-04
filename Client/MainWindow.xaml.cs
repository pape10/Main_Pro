using ChattingInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IChattingService Server;
        private static DuplexChannelFactory<IChattingService> _channelFactory;
        public string tgt = "";
        public MainWindow()
        {
            InitializeComponent();
            _channelFactory = new DuplexChannelFactory<IChattingService>(new ClientCallback(),"ChattingServiceEndPoint");
            Server = _channelFactory.CreateChannel();
        }
        public void TakeMessage(string message,string userName)
        {
            /*
            TextDisplayTextBox.Text += userName + ":" + message + "\n";
            TextDisplayTextBox.ScrollToEnd();
            */
        }
        public void GetSessionToken1()
        {
            if(tgt=="")
            {
                return;
            }
            string session = Server.GenerateSesionToken(tgt,"Spn1");
            string path = "c:\\Users\\Public\\KerbTray.txt";
            MessageBox.Show(session);
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine(session);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        public static string Encrypt(string input, string key1)
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
        public static string Decrypt(string input, string key1)
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


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserListBox.Items.Add("trying to login" + "\n");
            string auth = Encrypt(Convert.ToString(DateTime.Now), password.Text);
            string returnString = Server.KLogin(username.Text, auth);
            UserListBox.Items.Add("authenticator being sent is : "+ auth + "\n");
            if (returnString == "")
            {
                username.Text = "";
                password.Text = "";
                MessageBox.Show("error in logging in");
                UserListBox.Items.Add("error in authenticating" +"\n");
            }
            else
            {
                //MessageBox.Show(returnString);
                username.Text = "";
                password.Text = "";
                tgt = returnString;
                Login.IsEnabled = false;
                UserListBox.Items.Add("logged in" + "\n");
                UserListBox.Items.Add("tgt is : "+tgt + "\n");
                string path = "c:\\Users\\Public\\KerbTrayTGT.txt";
                
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);

                sw.WriteLine(tgt);
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            /*if(returnValue == 1 )
            {
                
            }

            else if(returnValue == 0)
            {

                MessageBox.Show("logged in");
                WelcomeText.Content = "Welcome" + username.Text;
                username.IsEnabled = false;
                Login.IsEnabled = false;
                LoadUserList(Server.GetCurrentUsers());
            }*/
            
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tgt = "";
            string path = "c:\\Users\\Public\\KerbTrayTGT.txt";

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("");
            sw.Flush();
            sw.Close();
            fs.Close();

            Server.Logout();
        }
        public void AddUserToList(string userName)
        {/*
            if(UserListBox.Items.Contains(userName))
            {
                return;
            }

            UserListBox.Items.Add(userName);
        */
            }
        public void RemoveUserToList(string userName)
        {
            /*
            if (UserListBox.Items.Contains(userName))
            {
                UserListBox.Items.Remove(userName);
            }
            */
        }
        private void LoadUserList(List<string> users)
        {
            /*
            foreach(var user in users)
            {
                AddUserToList(user);
            }*/
        }

        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
