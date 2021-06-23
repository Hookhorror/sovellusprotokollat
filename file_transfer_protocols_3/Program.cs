using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net;

namespace file_transfer_protocols_3
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("tftp server");
            string ownAddress = "127.0.0.1";
            int portToListen = 6969;
            string path = @"/home/joni/tftp/";
            string fileName = "alice.txt";
            Socket soc = null;
            Socket data = null;
            IPAddress host;

            // Vastaanota rrq pyyntö
            try
            {
                CreateListeninUdpSocket(ownAddress, portToListen, out soc, out host);
                // Kuuntele porttia 6969
                WaitForConnection(soc);

                IPEndPoint clientIep = new IPEndPoint(host, 0);
                EndPoint clientRemote = (EndPoint)clientIep;
                byte[] rec = new byte[516];
                int howMany;

                howMany = soc.ReceiveFrom(rec, ref clientRemote);

                fileName = ParseFilename(rec);

                // byte[] testi = Encoding.UTF8.GetBytes("testi");
                data = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                SendFile(path, fileName, data, clientRemote);
                // data.SendTo(testi, clientRemote);
                // Ota talteen asiakkaan osoite ja portti

                System.Console.WriteLine("jee");
                // Kun asiakas löydetty paiskaa heti paketilla jostain portista 1000 - 99999
            }
            finally
            {
                data.Close();
                soc.Close();
            }
        }

        private static string ParseFilename(byte[] rec)
        {
            string name = "";
            int howMany = 0;
            int i = 2;
            while (!rec[i].Equals((byte)0))
            {
                howMany++;
                i++;
            }
            name = Encoding.UTF8.GetString(rec, 2, howMany);

            return name;
        }

        private static void SendFile(string path, string fileName, Socket data, EndPoint clientRemote)
        {
            byte[] rec = new byte[512];
            byte[] block = null;
            int blockLength = 0;
            int blockNumber = 1;
            using (FileStream fs = File.OpenRead(path + fileName))
            {
                do
                {
                    System.Threading.Thread.Sleep(10);
                    blockLength = fs.Read(rec, 0, 512);
                    block = new byte[4 + blockLength];
                    block[0] = (byte)0;
                    block[1] = (byte)3;
                    block[2] = (byte)(blockNumber / 256);
                    block[3] = (byte)(blockNumber % 256);

                    for (int i = 0; i < blockLength; i++)
                    {
                        block[i + 4] = rec[i];
                    }

                    data.SendTo(block, clientRemote);
                    blockNumber++;
                }
                while (block.Length == 516);
            }
        }


        private static void CreateListeninUdpSocket(string ownAddress, int portToListen, out Socket soc, out IPAddress host)
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            soc.ReceiveTimeout = 5000;
            host = IPAddress.Parse(ownAddress);
            IPEndPoint iep = new IPEndPoint(host, portToListen);
            EndPoint ep = (EndPoint)iep;
            soc.Bind(ep);
        }

        private static void WaitForConnection(Socket soc)
        {
            while (soc.Available <= 0)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
