using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

class Asiakas
{
    public NetworkStream ns = null;
    public StreamWriter sw = null;
    public StreamReader sr = null;
    public Socket soketti = null;


    public Asiakas() { }

    public void User(string nimi)
    {
        KirjoitaTietovirtaan("user " + nimi);
    }

    public void Pass(string pass)
    {
        KirjoitaTietovirtaan("pass " + pass);
    }

    public void Epsv(string parametrit)
    {
        KirjoitaTietovirtaan("epsv " + parametrit);
    }

    public void List(string parametrit)
    {
        KirjoitaTietovirtaan("list " + parametrit);
    }

    public void KirjoitaTietovirtaan(string teksti)
    {
        sw.WriteLine(teksti);
        sw.Flush();
    }

    public string LueRiviTietovirrasta()
    {
        return sr.ReadLine();
    }

    public string[] LueKaikkiRivit()
    {
        List<string> rivit = new List<string>();
        string rivi = "";

        using (StreamReader reader = sr)
        {
            while (!sr.EndOfStream)
            {
                if (sr.EndOfStream)
                {
                    break;
                }
                rivi = sr.ReadLine();
                rivit.Add(rivi);
            }

        }


        return rivit.ToArray();
    }

    public void TulostaRiviTietovirrasta()
    {
        System.Console.WriteLine(LueRiviTietovirrasta());
    }

    public void AvaaTietovirta()
    {
        ns = new NetworkStream(soketti);
        sw = new StreamWriter(ns);
        sr = new StreamReader(ns);
    }

    internal void Yhdista(string ipOsoite, int portti)
    {
        soketti.Connect(ipOsoite, portti);
    }

    public void LuoTcpSoketti()
    {
        soketti = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public int ParseLatausportti(string vastaus)
    {
        string muokattava = vastaus;
        string porttiString = "";
        int palautettava = -1;
        int mista = vastaus.IndexOf("|||") + 3;
        int montako = 5;

        porttiString = muokattava.Substring(mista, montako);

        int numero;
        if (Int32.TryParse(porttiString, out numero))
        {
            palautettava = numero;
        }

        return palautettava;
    }



    public void SuljeTietovirrat()
    {
        sr.Close();
        sw.Close();
        ns.Close();
    }

    public void SuljeSoketti()
    {
        soketti.Close();
    }
}