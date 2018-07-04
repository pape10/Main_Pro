using ChattingInterfaces;
using Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace resourceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IClient client = new ClientCallback();
            string ses_key = "pape-3pa8-sqoy19";
            
            HttpListener listener = null;
            try
            {
                
                listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:1300/simplekerberos/");
                listener.Start();
                

                while (true)
                {
                    Console.WriteLine("waiting..");
                    HttpListenerContext context = listener.GetContext();
                    string session = client.requestForSPN();
                    ResourceServer rs = new ResourceServer();

                    //string session = "";        
                    string user = "";
                    if (session != "")
                    {
                        try
                        {
                            rs.Decrypt(session, ses_key);
                            user = "verified";
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    string msg = "";
                    //client.requestForSPN();
                    if(user=="verified")
                        msg = "<center><b>"+"user is verified with Kerberos"+"<//b></center>"+"<br>";
                    else
                        msg = "<center><b>" + "user is not verified with Kerberos please go to login page" + "<//b></center>" + "<br>";
                    context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(msg);
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    using (Stream stream = context.Response.OutputStream)
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(msg);
                        }
                    }
                    string username = "";

                   
                    Console.WriteLine("msg sent...");

                }

            }
            catch(WebException e)
            {
                Console.WriteLine(e.Status);
            }
        }

        
    }
}
