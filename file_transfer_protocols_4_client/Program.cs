using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace file_transfer_protocols_4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("tftp client");
            Socket soc = null;
            string server = "127.0.0.1";
            int port = 6969;
            // int receivingPort = 0;
            string fileName = "alice.txt"; // ladattava tiedosto
            string mode = "octet"; // octet tai netascii
            string request = "RRQ"; // WRQ tai RRQ

            try
            {
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                soc.ReceiveTimeout = 5000;
                IPAddress host = IPAddress.Parse(server);
                IPEndPoint iep = new IPEndPoint(host, port);
                EndPoint ep = (EndPoint)iep;

                if (request.ToUpper() == "RRQ")
                {
                    soc.SendTo(RRQ(fileName, mode), ep);
                    ReceiveFileFromTo(host, soc, fileName);
                }
                if (request.ToUpper() == "WRQ")
                {
                    soc.SendTo(WRQ(fileName, mode), ep);
                    // Kuuntele uusi porttinumero
                    // CreateListeninUdpSocket("127.0.0.1", iep.Port, out soc, out host);
                    // tähän jäi
                    // WaitForAck(soc, ref ep);

                    SendFileTo(host, soc, fileName);
                }
            }
            finally
            {
                soc.Close();
            }


            // lue palvelimelta RRQ
            // ReadFromServer();

            // UdpYhteys asiakas = new UdpYhteys("localhost", 69);
        }

        private static void WaitForAck(Socket soc, ref EndPoint ep)
        {
            byte[] rec = new byte[4];
            soc.ReceiveFrom(rec, 0, ref ep);
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


        private static void SendFileTo(IPAddress host, Socket soc, string fileName)
        {
            IPEndPoint serverIep = new IPEndPoint(host, 0); // Vastaanotetun paketin lähettäjä
            EndPoint remote = (EndPoint)serverIep; // Typecast EndPoint:ksi
            byte[] buffer = new byte[516];
            int howMany;
            int blockNumber = 1;
            byte[] block = new byte[0];

            using (FileStream fs = File.OpenRead(@"./" + fileName))
            {
                do
                {
                    // howMany = soc.ReceiveFrom(buffer, ref remote);
                    WaitForAck(soc, ref remote);
                    // TODO: Tarkista block numero
                    // Vastaanottojutut tähän väliin
                    howMany = fs.Read(buffer, 0, 512);
                    block = new byte[4 + howMany];
                    block[0] = (byte)0;
                    block[1] = (byte)3;
                    block[2] = (byte)(blockNumber / 256);
                    block[3] = (byte)(blockNumber % 256);
                    for (int i = 0; i < howMany; i++)
                    {
                        block[4 + i] = buffer[i];
                    }

                    blockNumber++;

                    soc.SendTo(block, remote);
                }
                while (block.Length == 516);
            }

        }

        private static void ReceiveFileFromTo(IPAddress host, Socket soc, string fileName)
        {
            IPEndPoint sender = new IPEndPoint(host, 0); // Vastaanotetun paketin lähettäjä
            EndPoint senderRemote = (EndPoint)sender; // Typecast EndPoint:ksi
            byte[] rec = new byte[516];
            int howMany;
            // Silmukkajuttuja
            using (FileStream fs = File.OpenWrite(@"./" + fileName))
            {
                do
                {
                    howMany = soc.ReceiveFrom(rec, ref senderRemote);
                    // byte[] data = byte[](rec, 4, howMany);
                    fs.Write(rec, 4, howMany - 4);
                    soc.SendTo(Ack(rec, senderRemote), senderRemote);
                }
                while (howMany >= 516);

            }
        }

        private static byte[] Ack(byte[] rec, EndPoint senderRemote)
        {
            byte[] ack = new byte[4] { (byte)0, (byte)4, rec[2], rec[3] };
            return ack;
        }


        private static byte[] WRQ(string fileName, string mode)
        {
            byte[] fileNameArray = Encoding.UTF8.GetBytes(fileName);
            byte[] modeArray = Encoding.UTF8.GetBytes(mode);
            int size = 4 + fileNameArray.Length + modeArray.Length;
            byte[] rrq = new byte[size];

            rrq[0] = 0;
            rrq[1] = 2;
            // Lisätään tiedostonimi
            for (int i = 0; i < fileNameArray.Length; i++)
            {
                rrq[i + 2] = fileNameArray[i];
            }
            rrq[2 + fileNameArray.Length] = 0;
            // Lisätään mode
            for (int i = 0; i < modeArray.Length; i++)
            {
                rrq[i + 2 + fileNameArray.Length + 1] = modeArray[i];
            }
            rrq[2 + fileNameArray.Length + 1 + modeArray.Length] = 0;

            return rrq;
        }


        private static byte[] RRQ(string fileName, string mode)
        {
            byte[] fileNameArray = Encoding.UTF8.GetBytes(fileName);
            byte[] modeArray = Encoding.UTF8.GetBytes(mode);
            int size = 4 + fileNameArray.Length + modeArray.Length;
            byte[] rrq = new byte[size];

            rrq[0] = 0;
            rrq[1] = 1;
            // Lisätään tiedostonimi
            for (int i = 0; i < fileNameArray.Length; i++)
            {
                rrq[i + 2] = fileNameArray[i];
            }
            rrq[2 + fileNameArray.Length] = 0;
            // Lisätään mode
            for (int i = 0; i < modeArray.Length; i++)
            {
                rrq[i + 2 + fileNameArray.Length + 1] = modeArray[i];
            }
            rrq[2 + fileNameArray.Length + 1 + modeArray.Length] = 0;

            return rrq;
        }
    }
}
