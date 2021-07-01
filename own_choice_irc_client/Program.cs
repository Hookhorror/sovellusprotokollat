using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace own_choice_irc_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Irc client");

            // string serverAddress = "192.168.1.2";
            IPAddress serverAddress = IPAddress.Parse("192.168.1.2");
            int port = 6667;
            Socket soc = null;
            NetworkStream ns = null;
            StreamReader sr = null;
            StreamWriter sw = null;

            try
            {
                soc = CreateTcpSocket();
                EndPoint ep = CreateEndPoint(serverAddress, port);
                soc.Connect(ep);

                ns = new NetworkStream(soc);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);

                ReadAllLinesFromStream(sr);
                ReadAllLinesFromStream(sr);

                sw.WriteLine("PASS pass\r\n");
                sw.Flush();
                sw.WriteLine("NICK nick\r\n");
                sw.Flush();
                sw.WriteLine("USER username host server realusername\r\n");
                sw.Flush();

                ReadAllLinesFromStream(sr);

                sw.WriteLine("WHOIS nick\r\n");
                sw.Flush();
                ReadAllLinesFromStream(sr);

                sw.WriteLine("QUIT jeejeejepulis");
                sw.Flush();
                ReadAllLinesFromStream(sr);

            }
            finally
            {
                sw.Close();
                sr.Close();
                ns.Close();
                soc.Close();
            }
            // Avaa TCP yhteys

            // PASS

            // NICK

            // USER
        }

        private static EndPoint CreateEndPoint(IPAddress serverAddress, int port)
        {
            IPEndPoint iep = new IPEndPoint(serverAddress, port);
            EndPoint ep = (EndPoint)iep;
            return ep;
        }

        private static Socket CreateTcpSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static void ReadAllLinesFromStream(StreamReader sr)
        {
            string line = string.Empty;
            do
            {
                line = sr.ReadLine();
                System.Console.WriteLine(line);
            }
            while (sr.Peek() > -1);
        }
    }
}
