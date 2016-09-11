using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SocketServer
{ 
    class Program
    {  
        private static byte[] result = new byte[1024];
        static string path = @"C:\Socket";
        private static int myProt = 8080;   //端口  
        static Socket serverSocket;
        static string message ;
        static void Main(string[] args)
        {
            //服务器IP地址   
            //IPAddress ip = IPAddress.Parse("192.168.239.134");
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
            serverSocket.Listen(1);    //设定排队连接请求  
            Console.WriteLine("Start monitor {0} successfull", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            //Console.WriteLine(message);
            Thread.Sleep(9000);
            //serverSocket.Close();
            Console.ReadLine();

        }  
  
        /// <summary>  
        /// 监听客户端连接  
        /// </summary>  
        private static void ListenClientConnect()
        {  
            while (true)  
            {  
                Socket clientSocket = serverSocket.Accept();  
                //clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));  
                //Thread receiveThread = new Thread(ReceiveMessage);  
                //receiveThread.Start(clientSocket);
                if (clientSocket.Connected)
                {
                    //Thread cThread = new Thread(new ParameterizedThreadStart(ReceiveFile));
                    Thread cThread = new Thread(new ParameterizedThreadStart(ReceiveMessage));

                    cThread.IsBackground = true;
                    cThread.Start(clientSocket);
                }
            }  
        }

        /// <summary>
        /// ReceiveMessage
        /// </summary>
        /// <param name="clientSocket"></param>
        public static void ReceiveMessage(object clientSocket)
        {
            bool status = true;
            Socket myClientSocket = (Socket)clientSocket;
            string clientNmae = myClientSocket.RemoteEndPoint.ToString();
            Console.WriteLine("New client coming to send Message: " + clientNmae);
            int count = 2;
            while (count>0)
            {
                try
                {
                    int receiveMessage = myClientSocket.Receive(result);
                    message = Encoding.ASCII.GetString(result, 0, receiveMessage);
                    //Invoke MSTest.exe
                    InvokeMSTestbat(message);
                    Console.WriteLine("Receive client: {0}, message: {1}",myClientSocket.RemoteEndPoint.ToString(), message);
                    count--;
                    Thread.Sleep(9000);
                    //status = false;
                    //return message;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
            
        }

        /// <summary>  
        /// ReceiveFile  
        /// </summary>  
        /// <param name="clientSocket"></param>  
        private static void ReceiveFile(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            //int receiveNumber = myClientSocket.Receive(result);
            string clientName = myClientSocket.RemoteEndPoint.ToString();
            Console.WriteLine("New client coming to send File: " + clientName);
            try
            {
                while (true)
                {

                        //通过clientSocket接收数据 
                        byte[] buffer = new byte[1024];
                        int count = myClientSocket.Receive(buffer);
                        Console.WriteLine("Receive File from {0}: {1}", clientName, Encoding.Default.GetString(buffer, 0, count));
                        
                        string[] command = Encoding.Default.GetString(buffer, 0, count).Split(',');
                        string fileName;
                        long length;
                        if (command[0] == "namelength")
                        {
                            fileName = command[1];
                            length = Convert.ToInt64(command[2]);
                            myClientSocket.Send(Encoding.Default.GetBytes("OK"));
                            long receive = 0L;
                            Console.WriteLine("Receiveing file:" + fileName + ".Plz wait...");
                            using (FileStream writer = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                int received;
                                while (receive < length)
                                {
                                    received = myClientSocket.Receive(buffer);
                                    writer.Write(buffer, 0, received);
                                    writer.Flush();
                                    receive += (long)received;
                                }
                            }
                            Console.WriteLine("Receive finish.\n");
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                myClientSocket.Shutdown(SocketShutdown.Both);
                myClientSocket.Close();

            }
             
        }

        public static void InvokeMSTestbat(string message)
        {
            string commandMSText = @" /testcontainer:C:\DF-AUTO\DFLRAutoFx\bin\Debug\";
            string arguments = commandMSText + message;
            try
            {
                string VsDevCmd = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest";
                //string VsDevCmd = @"C:\DF-AUTO\DFLink Reload Automation\DFLRAutoFx\MSTest";
                Process proc = new Process();
                proc.StartInfo.FileName = VsDevCmd;
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.RedirectStandardInput = true;
                //proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.Arguments = arguments;
                proc.Start();
                //Console.ReadLine();
                proc.WaitForExit();
                //proc.StandardInput.WriteLine("MSTest /testcontainer:");
                //proc.StandardInput.AutoFlush = true;
                //proc.StandardOutput.ReadLine();
                //Process.Start(VsDevCmd, commandMSText);

                //string s = proc.StandardOutput.ReadToEnd();
                //Console.WriteLine(s);
                //Console.ReadLine();

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
            }

        }
    }  

}
