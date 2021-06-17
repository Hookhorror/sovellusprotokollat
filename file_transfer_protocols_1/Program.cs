using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace file_transfer_protocols_1
{
    class Program
    {
        private static int portti = 21;
        private static int latausPortti = -1;
        private static string[] tiedostoListaus = null;
        private static string tiedostot = "";
        private static byte[] tallennettava = new byte[0];
        private static string tiedostonimi = "testi";
        private static bool on = true;
        private static bool tallennetaan = false;
        private static NetworkStream nsData = null;
        public static bool dataLahetetty = false;
        public static bool retr = false;

        static void Main(string[] args)
        {
            Socket soketti = null;
            NetworkStream ns = null;
            StreamWriter sw = null;
            Socket sokettiData = null;
            string palvelimenVastaus = "";
            string tulkinta = "";
            string pyynto = "";

            try
            {
                soketti = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soketti.Connect("192.168.1.2", 21);
                ns = new NetworkStream(soketti);
                sw = new StreamWriter(ns);

                while (on)
                {
                    {
                        palvelimenVastaus = LuePalvelimenVastaus(ns);

                    }
                    if (tallennetaan && dataLahetetty)
                    {
                        tallennetaan = false;
                    }
                    pyynto = ToimiVastauksenPerusteella(palvelimenVastaus);
                    if (pyynto.Equals("list"))
                    {
                        sokettiData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        sokettiData.Connect("192.168.1.2", latausPortti);
                        nsData = new NetworkStream(sokettiData);
                    }
                    if (pyynto.StartsWith("retr"))
                    {
                        sokettiData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        sokettiData.Connect("192.168.1.2", latausPortti);
                        nsData = new NetworkStream(sokettiData);
                        tallennetaan = true;
                    }

                    LahetaPyynto(sw, pyynto);
                }
            }
            finally
            {
                nsData.Close();
                sokettiData.Close();
                sw.Close();
                ns.Close();
                soketti.Close();
            }
        }

        private static void TallennaTiedostoonTietovirrasta()
        {
            byte[] readBuffer = new byte[1024];
            int numberOfBytesRead = 0;
            string path = @"./" + tiedostonimi;

            using (FileStream fs = File.OpenWrite(path))
            {
                do
                {
                    numberOfBytesRead = nsData.Read(readBuffer, 0, readBuffer.Length);
                    fs.Write(readBuffer, 0, numberOfBytesRead);
                    System.Threading.Thread.Sleep(1);
                }
                while (nsData.DataAvailable);
            }
        }

        /* private static void TallennaTiedostoon()
        {
            string path = @"./" + tiedostonimi;

            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            //Create the file.
            using (FileStream fs = File.Create(path))
            {
                // byte[] kirjoitettava = new UTF8Encoding(true).GetBytes(tallennettava);
                fs.Write(tallennettava);
            }
        } */


        private static void LahetaPyynto(StreamWriter sw, string pyynto)
        {
            sw.WriteLine(pyynto);
            sw.Flush();
            System.Console.WriteLine("Asiakas:  " + pyynto);
        }

        private static string ToimiVastauksenPerusteella(string palvelimenVastaus)
        {
            dataLahetetty = false;
            if (palvelimenVastaus.StartsWith("220")) return "user anonymous";
            if (palvelimenVastaus.StartsWith("331")) return "pass ";
            if (palvelimenVastaus.StartsWith("230")) return "epsv";
            if (palvelimenVastaus.StartsWith("229"))
            {
                latausPortti = ParseLatausportti(palvelimenVastaus);
                System.Console.WriteLine("Käytä komentoja list, retr tai quit");
                string komento = Console.ReadLine();
                tiedostonimi = ParseTiedostonimi(komento);
                // return "list";
                return komento;
            } // Avaa uusi yhteys
            if (palvelimenVastaus.StartsWith("150")) // lähettämässä jotain
            {
                if (tallennetaan)
                {
                    TallennaTiedostoonTietovirrasta();
                }
                else
                    tiedostot = LuePalvelimenVastaus(nsData);
                dataLahetetty = true;
                return "";
            }
            if (palvelimenVastaus.StartsWith("226")) // lähetys valmis
            {
                System.Console.WriteLine("Käytä komentoja epsv tai quit");
                string ladatattava = Console.ReadLine();
                // on = false;
                return ladatattava;
            }
            if (palvelimenVastaus.StartsWith("221")) on = false;

            return "";
        }

        private static string ParseTiedostonimi(string komento)
        {
            string palautettava = "testi";
            string[] parametrit = komento.Split(' ');
            if (parametrit.Length > 1)
            {
                palautettava = parametrit[1];
            }
            return palautettava;
        }

        private static string TulkitsePalvelimenVastaus(string palvelimenVastaus)
        {
            return palvelimenVastaus.Split(' ')[0];
        }

        private static string LuePalvelimenVastaus(NetworkStream ns)
        {
            if (ns.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;

                // Incoming message may be larger than the buffer size.
                do
                {
                    numberOfBytesRead = ns.Read(myReadBuffer, 0, myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (ns.DataAvailable);

                // Print out the received message to the console.
                // if (tallennetaan)
                // {
                //     TallennaTiedostoonTietovirrasta();
                // }
                // else
                {
                    Console.Write("Palvelin: " +
                                                 myCompleteMessage);
                }

                return myCompleteMessage.ToString();
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");

                return "";
            }

        }

        public static int ParseLatausportti(string vastaus)
        {
            string muokattava = vastaus;
            string porttiString = "";
            int palautettava = -1;
            int mista = vastaus.IndexOf("|||") + 3;
            int mihin = muokattava.IndexOf('|', mista, 6);
            int montako = mihin - mista;

            porttiString = muokattava.Substring(mista, montako);

            int numero;
            if (Int32.TryParse(porttiString, out numero))
            {
                palautettava = numero;
            }

            return palautettava;
        }
    }
}
