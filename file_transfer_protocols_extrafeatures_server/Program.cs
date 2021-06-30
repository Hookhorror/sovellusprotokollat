using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Net;

namespace file_transfer_protocols_extrafeatures_server
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
            string request = "";
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
                request = ParseRequest(rec);
                fileName = ParseFilename(rec);

                data = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                if (request == "RRQ")
                {
                    SendFile(path, fileName, data, clientRemote);
                }
                if (request == "WRQ")
                {
                    ReceiveFileFromTo(host, data, fileName, clientRemote);
                }
                // Ota talteen asiakkaan osoite ja portti

                // Kun asiakas löydetty paiskaa heti paketilla jostain portista 1000 - 99999
            }
            finally
            {
                data.Close();
                soc.Close();
            }
        }

        private static string ParseRequest(byte[] rec)
        {
            if (rec[1].Equals((byte)1))
            {
                return "RRQ";
            }
            if (rec[1].Equals((byte)2))
            {
                return "WRQ";
            }

            return "ERROR";
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


        private static void ReceiveFileFromTo(IPAddress host, Socket soc, string fileName, EndPoint clientRemote)
        {
            // IPEndPoint sender = new IPEndPoint(host, 0); // Vastaanotetun paketin lähettäjä
            // EndPoint senderRemote = (EndPoint)sender; // Typecast EndPoint:ksi
            byte[] rec = new byte[516];
            int howMany;
            int blockNumber = 0;
            // Silmukkajuttuja
            using (FileStream fs = File.OpenWrite(@"./" + fileName))
            {
                do
                {
                    // soc.Bind(senderRemote); // Lisätty
                    soc.SendTo(AckToWrq(rec, blockNumber), clientRemote);
                    System.Threading.Thread.Sleep(5); // testi
                    howMany = soc.ReceiveFrom(rec, ref clientRemote);
                    // byte[] data = byte[](rec, 4, howMany);
                    fs.Write(rec, 4, howMany - 4);
                    // soc.SendTo(Ack(rec, senderRemote), senderRemote);
                    blockNumber++;
                }
                while (howMany >= 516);

            }
        }

        private static byte[] AckToWrq(byte[] rec, int block)
        {
            byte[] ack = new byte[4] { (byte)0, (byte)4, (byte)(block / 256), (byte)(block % 256) };
            return ack;
        }

        private static byte[] Ack(byte[] rec, EndPoint senderRemote)
        {
            byte[] ack = new byte[4] { (byte)0, (byte)4, rec[2], rec[3] };
            return ack;
        }


        private static void SendFile(string path, string fileName, Socket data, EndPoint clientRemote)
        {
            byte[] rec = new byte[512];
            byte[] block = null;
            byte[] lastBlockSent = null;
            int blockLength = 0;
            int blockNumber = 0;
            bool ackIsCorrect = true;
            using (FileStream fs = File.OpenRead(path + fileName))
            {
                do
                {
                    // System.Threading.Thread.Sleep(10);
                    // TODO: tarkista viimeisin blokki
                    // Lue saatu viesti, tarkista ack, jos oikein niin jatka
                    if (blockNumber > 0)
                    {
                        ackIsCorrect = CheckForCorrectAck(blockNumber, data, clientRemote);
                    }
                    if (ackIsCorrect)
                    {
                        blockNumber++;

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
                        lastBlockSent = block;
                        data.SendTo(block, clientRemote);
                    }
                    else
                    {
                        data.SendTo(lastBlockSent, clientRemote);
                    }
                }
                while (block.Length == 516);
            }
        }

        private static bool CheckForCorrectAck(int blockNumber, Socket data, EndPoint ep)
        {
            byte[] expected = { (byte)0, (byte)4, (byte)(blockNumber / 256), (byte)(blockNumber % 256) };
            byte[] rec = new byte[4];
            int howMany = data.ReceiveFrom(rec, ref ep);

            for (int i = 0; i < rec.Length; i++)
            {
                if (rec[i].Equals(expected[i]))
                {
                    continue;
                }
                else return false;
            }

            return true;
        }

        private static void CreateListeninUdpSocket(string ownAddress, int portToListen, out Socket soc, out IPAddress host)
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            soc.ReceiveTimeout = 5000;
            host = IPAddress.Parse(ownAddress);
            // IPEndPoint iep = new IPEndPoint(host, portToListen);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, portToListen);
            EndPoint ep = (EndPoint)iep;
            // soc.Bind(ep);
            soc.Bind(iep);
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
