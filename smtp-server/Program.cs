using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace smtp_server
{
    class Program
    {
        static bool on = true;
        static string palvelimenNimi = "Jonin palvelin";
        static bool edellinenOliData = false;
        static Tilat tila;

        static void Main(string[] args)
        {
            // Luo soketti
            Socket sokettiPalvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Asetetaan palvelin kuuntelemaan, johon se tarvii osoitteen
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 25000);
            sokettiPalvelin.Bind(iep);
            sokettiPalvelin.Listen(5);

            // Vastaanotetaan porttiin 25000 tuleva asiakas
            Socket sokettiAsiakas = sokettiPalvelin.Accept();
            tila = Tilat.YhteysMuodostettu;
            IPEndPoint iap = (IPEndPoint)sokettiAsiakas.RemoteEndPoint;
            Console.WriteLine("Yhteys osoitteesta {0} portissa {1}", iap.Address, iap.Port);
            NetworkStream ns = new NetworkStream(sokettiAsiakas);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            // Eka viesti
            sw.WriteLine("220 " + palvelimenNimi);
            sw.Flush();
            // asiakkaan palvelua while silmukassa
            while (on)
            {
                string asiakkaanViesti = sr.ReadLine();
                string vastausAsiakkaalle = tulkitseAsiakkaanViesti(asiakkaanViesti);
                sw.WriteLine(vastausAsiakkaalle);
                sw.Flush();
                OdotaAsiakkaanVastausta(asiakkaanViesti, sr);
            }
        }

        private static void OdotaAsiakkaanVastausta(string viimeisinViesti, StreamReader sr)
        {
            string asiakkaanViesti = "";
            while (asiakkaanViesti.Equals(viimeisinViesti))
            {
                asiakkaanViesti = sr.ReadLine();
                Thread.Sleep(100);
            }
        }

        public static string tulkitseAsiakkaanViesti(string vastaanotettu)
        {
            string lahetettava = "";
            string tulkittava = "";
            Console.Write(vastaanotettu);
            // Toteuta
            string[] status = vastaanotettu.Split(' ');

            if (tila.Equals(Tilat.Data))
            {
                tulkittava = status[status.Length - 1];
            }
            else
                tulkittava = status[0];


            switch (status[0])
            {
                case "HELO":
                    if (tila.Equals(Tilat.YhteysMuodostettu))
                    {
                        lahetettava = "250 " + palvelimenNimi + ", Hello there!";
                        tila = Tilat.Helo;
                    }
                    else lahetettava = "Kokeile MAIL TO";
                    break;
                case "MAIL":
                    if (tila.Equals(Tilat.Helo))
                    {
                        lahetettava = "250 ok";
                        tila = Tilat.Mail;
                    }
                    break;
                case "RCPT":
                    if (tila.Equals(Tilat.Mail))
                    {
                        lahetettava = "250 ok";
                        tila = Tilat.Rcpt;
                    }
                    break;
                case "DATA":
                    if (tila.Equals(Tilat.Rcpt))
                    {
                        // edellinenOliData = true;
                        lahetettava = "354 send the mail data, end with '.' on its own line";
                        tila = Tilat.Data;
                    }
                    break;
                case ".":
                    if (tila.Equals(Tilat.Data))
                    {
                        lahetettava = "250 ok";
                        // edellinenOliData = false;
                        tila = Tilat.Quit;
                    }
                    break;
                case "QUIT":
                    lahetettava = "221 Bye";
                    on = false;
                    break;
                default:
                    lahetettava = "500 Syntax error, command unrecognized";
                    break;
            }

            Console.WriteLine(lahetettava);
            return lahetettava;
        }

        enum Tilat
        {
            YhteysMuodostettu,
            Helo,
            Mail,
            Rcpt,
            Data,
            Quit
        }
    }
}
