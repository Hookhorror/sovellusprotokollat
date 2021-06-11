// using System;
// using System.IO;
// using System.Text;
// using System.Net;
// using System.Net.Sockets;

// namespace mail_protocols_2
// {
//     class Program
//     {
//         private static bool on = true;
//         private static Tila tila = Tila.oletus;
//         private static string user = "USER mt";
//         private static string pass = "PASS mt";

//         static void Main(string[] args)
//         {
//             string vastaanotettu = "";

//             Asiakas asiakas = new Asiakas(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//             // Lisää asiakkaalle on omat tietovirrat
//             asiakas.liita("localhost", 110);
//             NetworkStream ns = new NetworkStream(asiakas.soketti);
//             StreamReader sr = new StreamReader(ns);
//             StreamWriter sw = new StreamWriter(ns);


//             while (on)
//             {
//                 vastaanotettu = viestiMerkkijonoksi(sr);
//                 System.Console.WriteLine(vastaanotettu);
//                 // Lähetetään vastaus
//                 sw.WriteLine(VastausViestiin(vastaanotettu));
//                 sw.Flush();
//             }

//             vastaanotettu = asiakas.sr.ReadLine();
//             System.Console.WriteLine(vastaanotettu);

//             asiakas.sw.WriteLine(user);
//             asiakas.sw.Flush();



//             // Yhteys avattu: +OK POP3 server ready

//             vastaanotettu = asiakas.sr.ReadLine();
//             System.Console.WriteLine(vastaanotettu);

//             asiakas.sw.WriteLine(pass);
//             asiakas.sw.Flush();
//             // // Yhteys avattu: +OK POP3 server ready

//             vastaanotettu = asiakas.sr.ReadLine();
//             System.Console.WriteLine(vastaanotettu);

//             asiakas.sw.WriteLine("list");
//             asiakas.sw.Flush();
//             // // Yhteys avattu: +OK POP3 server ready
//             System.Threading.Thread.Sleep(10);
//             vastaanotettu = viestiMerkkijonoksi(asiakas.sr);
//             System.Console.WriteLine(vastaanotettu);

//             asiakas.sw.WriteLine("retr 1");
//             asiakas.sw.Flush();
//             // // Yhteys avattu: +OK POP3 server ready

//             vastaanotettu = viestiMerkkijonoksi(asiakas.sr);
//             System.Console.WriteLine(vastaanotettu);
//         }

//         private static string VastausViestiin(string vastaanotettu)
//         {
//             string vastaus = TulkitseViesti(vastaanotettu);
//             return vastaus;
//         }

//         private static string TulkitseViesti(string vastaanotettu)
//         {
//             string[] viestiPaloina = vastaanotettu.Trim().Split(' ', 2);
//             string tulkinta = "";
//             switch (viestiPaloina[0].ToUpper())
//             {
//                 case "+OK":
//                     switch (tila)
//                     {
//                         case Tila.oletus:
//                             tila = Tila.yhteysOtettu;
//                             break;
//                         case Tila.yhteysOtettu:
//                             tulkinta = user;
//                             tila = Tila.kayttajanimiAnnettu;
//                             break;
//                         case Tila.kayttajanimiAnnettu:
//                             tulkinta = pass;
//                             tila = Tila.kirjauduttu;
//                             break;
//                         case Tila.kirjauduttu:
//                             tulkinta = "LIST";
//                             break;
//                         default:
//                             // tila = Tila.yhteysOtettu;
//                             tulkinta = "";
//                             break;
//                     } // Switch +OK

//                     // if (tila.Equals(Tila.yhteysOtettu))
//                     // {
//                     //     tulkinta = user;
//                     //     tila = Tila.kayttajanimiAnnettu;
//                     // }

//                     // switch (viestiPaloina[1])
//                     // {
//                     //     case "POP3 server ready":
//                     //     default:
//                     //         tulkinta = user;
//                     //         break;
//                     // } // switch case +OK
//                     break;
//                 case "-ERR":
//                     tulkinta = "Virhe";
//                     break;
//                 case ".\r\n":
//                     tulkinta = "QUIT";
//                     on = false;
//                     break;
//                 default:
//                     tulkinta = "";
//                     break;
//             } // switch

//             return tulkinta;
//         }

//         private static string viestiMerkkijonoksi(StreamReader sr)
//         {
//             StringBuilder palautettava = new StringBuilder();
//             string luettu = "";
//             string lopetus = ".";
//             if (tila.Equals(Tila.kirjauduttu))
//             // while (!luettu.Equals("."))
//             {
//                 while (!luettu.Equals(lopetus))
//                 {
//                     luettu = sr.ReadLine();
//                     palautettava.Append(luettu);
//                     palautettava.Append("\r\n");
//                 }
//             }
//             return palautettava.ToString();
//         }

//         enum Tila
//         {
//             yhteysOtettu,
//             kirjauduttu,
//             kayttajanimiAnnettu,
//             oletus
//         }
//     }
// }
