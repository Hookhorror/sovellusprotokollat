using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
class Asiakas
{
    public Socket soketti = null;
    public NetworkStream ns = null;
    public StreamReader sr = null;
    public StreamWriter sw = null;
    public Asiakas(AddressFamily osoitetyyppi, SocketType sokettityyppi, ProtocolType protokolla)
    {
        soketti = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    }


    public void Liita(string osoite, int portti)
    {
        soketti.Connect(osoite, portti);
        ns = new NetworkStream(soketti);
        sr = new StreamReader(ns);
        sw = new StreamWriter(ns);
    }

    public void LahetaViesti(string viesti)
    {
        sw.WriteLine(viesti);
        sw.Flush();
        // System.Console.WriteLine(viesti);
    }

    public string VastaanotaViesti()
    {
        string vastaanotettu = sr.ReadLine();
        // System.Console.WriteLine(vastaanotettu);

        return vastaanotettu;
    }

    public string VastaanotaMonirivinenViesti()
    {
        // string vastaanotettu = sr.ReadLine();
        StringBuilder vastaanotettu = new StringBuilder();
        string ekaRivi = sr.ReadLine();
        vastaanotettu.Append(ekaRivi + "\r\n");
        string rivi;
        while ((rivi = sr.ReadLine()) != null)
        {
            vastaanotettu.Append(rivi + "\r\n");
            if (rivi.Equals("."))
            {
                break;
            }
        }

        return vastaanotettu.ToString();
    }

}