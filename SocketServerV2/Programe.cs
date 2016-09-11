using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml;
using System.IO;


namespace SocketClient
{
    class Program
    {
        private static byte[] size = new byte[1024];
        static Socket clientSocket;

        static void Main(string[] args)
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("192.168.239.134");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8080)); //配置服务器IP与端口  
                Console.WriteLine("Connect server successful");
            }
            catch
            {
                Console.WriteLine("Connect server fail，please press Enter to quite！");
                return;
            }
            Thread myThread = new Thread(SendMessage);
            myThread.Start();
            #region SendFile
            ////通过 clientSocket 发送文件
            //while (true)
            //{
            //    Console.WriteLine("please input the path of the file which you want to send：");
            //    string path = Console.ReadLine();
            //    try
            //    {
            //        using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            //        {
            //            long send = 0L, length = reader.Length;
            //            string sendStr = "namelength," + Path.GetFileName(path) + "," + length.ToString();


            //            string filename = Path.GetFileName(path);
            //            clientSocket.Send(Encoding.Default.GetBytes(sendStr));

            //            //通过clientSocket接收数据  
            //            int receiveLength = clientSocket.Receive(size);
            //            string mes = Encoding.ASCII.GetString(size, 0, receiveLength);
            //            Console.WriteLine("Receive from server message：{0}", mes);
            //            int buffersize = 1024;
            //            if (mes.Contains("OK"))
            //            {
            //                Console.WriteLine("Sending file:" + filename + ".Plz wait...");
            //                byte[] filebuffer = new byte[buffersize];
            //                int read, sent;
            //                while ((read=reader.Read(filebuffer,0,buffersize))!=0)
            //                {
            //                    sent = 0;
            //                    while((sent+= clientSocket.Send(filebuffer, sent, read, SocketFlags.None))<read)
            //                    {
            //                        send += (long)sent;
            //                    }
            //                }
            //                Console.WriteLine("Send finish.\n");
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}
            #endregion
            #region sendMessage
            //通过 clientSocket 发送数据
            //for (int i = 0; i < 10; i++)
            //{
            //    try
            //    {
            //        Thread.Sleep(1000);    //等待1秒钟  
            //        string sendMessage = "client send Message Hello " + DateTime.Now;
            //        Console.WriteLine("Please input your message to Server");
            //        string sendMessage1 = Console.ReadLine();
            //        clientSocket.Send(Encoding.Default.GetBytes(sendMessage1));
            //        Console.WriteLine("Send message to server：{0} ", sendMessage1);
            //    }
            //    catch
            //    {
            //        clientSocket.Shutdown(SocketShutdown.Both);
            //        clientSocket.Close();
            //        break;
            //    }
            //}
            #endregion
            Console.WriteLine("Send successful，press Enter to quit");
            Console.ReadLine();
        }

        public static void SendFile()
        {
            Socket currectSocket = (Socket)clientSocket;
            while (true)
            {
                Console.WriteLine("please input the path of the file which you want to send：");
                string path = Console.ReadLine();
                try
                {
                    using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        long send = 0L, length = reader.Length;
                        string sendStr = "namelength," + Path.GetFileName(path) + "," + length.ToString();


                        string filename = Path.GetFileName(path);
                        currectSocket.Send(Encoding.Default.GetBytes(sendStr));

                        //通过clientSocket接收数据  
                        int receiveLength = currectSocket.Receive(size);
                        string mes = Encoding.ASCII.GetString(size, 0, receiveLength);
                        Console.WriteLine("Receive from server message：{0}", mes);
                        int buffersize = 1024;
                        if (mes.Contains("OK"))
                        {
                            Console.WriteLine("Sending file:" + filename + ".Plz wait...");
                            byte[] filebuffer = new byte[buffersize];
                            int read, sent;
                            while ((read = reader.Read(filebuffer, 0, buffersize)) != 0)
                            {
                                sent = 0;
                                while ((sent += currectSocket.Send(filebuffer, sent, read, SocketFlags.None)) < read)
                                {
                                    send += (long)sent;
                                }
                            }
                            Console.WriteLine("Send finish.\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public static void SendMessage()
        {
            Socket currectSocket = (Socket)clientSocket;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Thread.Sleep(1000);    //等待1秒钟  
                    string sendMessage = "client send Message Hello " + DateTime.Now;
                    Console.WriteLine("Please input your selected DLL name to Server");
                    string sendMessage1 = Console.ReadLine();
                    currectSocket.Send(Encoding.Default.GetBytes(sendMessage1));
                    Console.WriteLine("Send message to server：{0} ", sendMessage1);
                }
                catch
                {
                    currectSocket.Shutdown(SocketShutdown.Both);
                    currectSocket.Close();
                    break;
                }
            }
            Console.WriteLine("Send successful，press Enter to quit");
            Console.ReadLine();
        }
        private static void SendXML(XmlDocument xmlFile)
        {
            Socket currectSocket = (Socket)clientSocket;
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            //Ignore the comments in XML
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create("@DFLink.xml", settings);
            xmlDoc.Load(reader);

            XmlNode xn = xmlDoc.SelectSingleNode("DFLinkSettings");
            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = xnf as XmlElement;
            }

        }
    }


}
