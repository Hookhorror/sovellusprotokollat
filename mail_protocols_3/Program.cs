using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;

namespace mail_protocols_3
{
    class Program
    {
        private const int smtpPortti = 25000;
        private const int pop3Portti = 25001;

        static bool on = true;
        static bool asiakkaanTyyppiSelvitetty = false;
        static string palvelimenNimi = "Jonin palvelin";
        // static bool edellinenOliData = false;
        static Tilat tila;
        static Inbox inbox = new Inbox();
        static StringBuilder emailViesti;
        static bool viestiTallennettu = false;

        static void Main(string[] args)
        {
            TcpListener smtpKuuntelija = OdotaYhteytta(); //TESTI
            smtpKuuntelija.AcceptSocket();                // TESTI
            // Socket smtpPalvelin = LuoSokettiJaKuuntele(smtpPortti);
            // Socket pop3Palvelin = LuoSokettiJaKuuntele(pop3Portti);
            Socket asiakas = LuoSokettiJaVastaanotaAsiakas(smtpPalvelin);
            // Avataan tietovirta palvelimen ja asiakkaan välille
            NetworkStream ns = new NetworkStream(asiakas);
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            // Oletetaan että kirjautuminen onnistui ja ilmoitetaan siitä asiakkaalle
            sw.WriteLine("220 " + palvelimenNimi);
            sw.Flush();

            // asiakkaan palvelua while silmukassa
            VastaanotaSmtpViesti(sr, sw);
        }

        private static TcpListener OdotaYhteytta()
        {
            TcpListener kuuntelija = new TcpListener(IPAddress.Loopback, smtpPortti);
            kuuntelija.Start();
            while (!kuuntelija.Pending())
            {
                Thread.Sleep(100);
            }

            return kuuntelija;
        }

        private static void VastaanotaSmtpViesti(StreamReader sr, StreamWriter sw)
        {
            while (on)
            {
                string asiakkaanViesti = sr.ReadLine();
                string vastausAsiakkaalle = tulkitseAsiakkaanViesti(asiakkaanViesti);
                sw.WriteLine(vastausAsiakkaalle);
                sw.Flush();
                OdotaAsiakkaanVastausta(asiakkaanViesti, sr);
                OtaRiviTalteen(asiakkaanViesti);
                TallennaViesti();
            }
        }

        private static Socket LuoSokettiJaVastaanotaAsiakas(Socket palvelin)
        {
            // Luodaaan soketti ja vastaanotetaan porttiin 25000 tuleva asiakas
            Socket asiakas = palvelin.Accept();
            tila = Tilat.YhteysMuodostettu;
            IPEndPoint iap = (IPEndPoint)asiakas.RemoteEndPoint;
            Console.WriteLine("Yhteys osoitteesta {0} portissa {1}", iap.Address, iap.Port);
            return asiakas;
        }

        private static Socket LuoSokettiJaKuuntele(int porttinro)
        {
            // Luo soketti
            Socket sokettiPalvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Asetetaan palvelin kuuntelemaan, johon se tarvii osoitteen
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, porttinro);
            sokettiPalvelin.Bind(iep);
            sokettiPalvelin.Listen(5);
            return sokettiPalvelin;
        }

        private static void OtaRiviTalteen(string asiakkaanViesti)
        {
            if (tila.Equals(Tilat.Data) && emailViesti != null)
            {
                emailViesti.Append(asiakkaanViesti + "\r\n");
            }
            if (tila.Equals(Tilat.Data) && emailViesti == null)
            {
                emailViesti = new StringBuilder();
            }
        }

        private static void TallennaViesti()
        {

            if (tila.Equals(Tilat.Quit) && emailViesti != null && !viestiTallennettu)
            {
                emailViesti.Append(".\r\n");
                inbox.Add(new Email(emailViesti.ToString()));
                viestiTallennettu = true;
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
            // string tulkittava = "";
            Console.Write(vastaanotettu);
            // Toteuta
            string[] status = vastaanotettu.Split(' ');

            // if (tila.Equals(Tilat.Data))
            // {
            //     tulkittava = status[status.Length - 1];
            // }
            // else
            //     tulkittava = status[0];


            switch (status[0].ToUpper())
            {
                // POP3
                case "USER":
                    lahetettava = "+OK";
                    break;
                case "PASS":
                    lahetettava = "+OK logged in";
                    break;
                case "LIST":
                    lahetettava = "+OK " + inbox.GetCount() + " messages:";
                    // Listaa viestit
                    break;
                case "RETR":
                    // Hae viesti
                    // lahetettava = "+OK";
                    break;
                // POP3
                // SMTP 
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
                    if (!tila.Equals(Tilat.Data))
                    {
                        lahetettava = "500 Syntax error, command unrecognized";
                    }
                    break;
                    // SMTP
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
