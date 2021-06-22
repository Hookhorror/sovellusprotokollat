using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace file_transfer_protocols_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("tftp client");
            Socket soc = null;
            string server = "127.0.0.1";
            int port = 6969;
            string fileName = "testi1.txt"; // ladattava tiedosto
            string mode = "octet"; // octet tai netascii

            try
            {
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                soc.ReceiveTimeout = 5000;
                IPAddress host = IPAddress.Parse(server);
                IPEndPoint iep = new IPEndPoint(host, port);
                EndPoint ep = (EndPoint)iep;

                soc.SendTo(RRQ(fileName, mode), ep);
                ReceiveFileFromTo(host, soc, fileName);
            }
            finally
            {
                soc.Close();
            }


            // lue palvelimelta RRQ
            // ReadFromServer();

            // UdpYhteys asiakas = new UdpYhteys("localhost", 69);
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
