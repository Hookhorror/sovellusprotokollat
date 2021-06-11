using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace mail_protocols_2
{
    class Program
    {
        private const string user = "USER mt";
        private const string pass = "PASS mt";
        // private static bool on = true;

        static void Main(string[] args)
        {
            string palvelimenVastaus = "";
            // string tulkinta = "";
            Asiakas asiakas = new Asiakas(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            asiakas.Liita("localhost", 110); // Avaa streamin
            palvelimenVastaus = asiakas.VastaanotaViesti();

            System.Console.WriteLine(palvelimenVastaus);
            asiakas.LahetaViesti("user mt");
            palvelimenVastaus = asiakas.VastaanotaViesti();

            System.Console.WriteLine(palvelimenVastaus);
            asiakas.LahetaViesti("pass mt");
            palvelimenVastaus = asiakas.VastaanotaViesti();

            System.Console.WriteLine(palvelimenVastaus);
            asiakas.LahetaViesti("list");
            palvelimenVastaus = asiakas.VastaanotaMonirivinenViesti();

            System.Console.WriteLine(palvelimenVastaus);
            asiakas.LahetaViesti("quit");
            palvelimenVastaus = asiakas.VastaanotaViesti();

            System.Console.WriteLine(palvelimenVastaus);
        }

        private static string TulkitseSaatuViesti(string palvelimenVastaus, StreamReader sr)
        {
            string[] vastaukset = palvelimenVastaus.Split(' ');
            StringBuilder tulkinta = new StringBuilder();
            switch (vastaukset[0])
            {
                case "+OK":
                    if (vastaukset[vastaukset.Length - 1].Equals(".")) // viimeinen sana piste
                    {
                        string rivi = "";
                        while (!rivi.Equals("."))
                        {
                            rivi = sr.ReadLine();
                            tulkinta.Append(rivi + "\r\n");
                        }
                    }
                    else tulkinta.Append(vastaukset[0]);
                    // Console.ReadLine();
                    break;
                case "-ERR":
                    System.Console.WriteLine("Virhe");
                    // Console.ReadLine();
                    break;
                default:
                    // tulkinta.Append("USER mt");
                    // Console.ReadLine();
                    break;
            } // Switch

            return tulkinta.ToString();
        }
    }
}