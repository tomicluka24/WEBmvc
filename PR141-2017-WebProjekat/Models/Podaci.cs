﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace PR141_2017_WebProjekat.Models
{
    public class Podaci
    {
        public static List<Korisnik> IscitajKorisnike(string putanja)
        {
            List<Korisnik> korisnici = new List<Korisnik>();
            putanja = HostingEnvironment.MapPath(putanja);
            FileStream stream = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string red = "";
            while ((red = sr.ReadLine()) != null)
            {
                string[] tokeni = red.Split(';');
                //Korisnik k = new Korisnik(tokeni[0], tokeni[1], tokeni[2], tokeni[3], tokeni[4], DateTime.ParseExact(tokeni[5], "MM/dd/yyyy", null), tokeni[6], false); 
                //korisnici.Add(k);

            if (tokeni[6] == "administrator")
            {
                Korisnik k = new Korisnik(tokeni[0], tokeni[1], tokeni[2], tokeni[3], tokeni[4], DateTime.ParseExact(tokeni[5], "MM/dd/yyyy", null), tokeni[6], bool.Parse(tokeni[7]));
                korisnici.Add(k);
            }
            else if (tokeni[6] == "kupac")
            {
                Dictionary<string, Karta> karte = IscitajKarte("~/App_Data/karte.txt");
                Dictionary<string, Karta> sveKarteKupca = new Dictionary<string, Karta>();

                string[] IDevi = tokeni[8].Split(',');

                for (int i = 0; i < IDevi.Length; i++)
                {
                    if (karte.ContainsKey(IDevi[i]))
                    {
                        sveKarteKupca.Add(IDevi[i], karte[IDevi[i]]); 
                    }
                }

                TipKorisnika tK = new TipKorisnika();
                tK.ImeTipa = tokeni[10];
                if (tK.ImeTipa == "zlatni")
                {
                    tK.Popust = 100;
                    tK.TrazeniBrojBodova = 3001;
                }
                else if (tK.ImeTipa == "srebrni")
                {
                    tK.Popust = 50;
                    tK.TrazeniBrojBodova = 2000;
                }
                else
                {
                    tK.Popust = 20;
                    tK.TrazeniBrojBodova = 500;
                }

                Korisnik k = new Korisnik(tokeni[0], tokeni[1], tokeni[2], tokeni[3], tokeni[4], DateTime.ParseExact(tokeni[5], "MM/dd/yyyy", null), tokeni[6], sveKarteKupca, double.Parse(tokeni[9]), tK, bool.Parse(tokeni[7]));
                korisnici.Add(k);
            }
            else
            {
                List<Manifestacija> manifestacije = IscitajManifestacije("~/App_Data/manifestacije.txt");
                List<Manifestacija> manifestacijeProdavca = new List<Manifestacija>();

                string[] nazivi = tokeni[8].Split(',');
                for (int i = 0; i < nazivi.Length; i++)
                {
                    foreach (Manifestacija m in manifestacije)
                    {
                        if (m.Naziv == nazivi[i])
                        {
                            manifestacijeProdavca.Add(m);
                        }
                    }
                }

                Korisnik k = new Korisnik(tokeni[0], tokeni[1], tokeni[2], tokeni[3], tokeni[4], DateTime.ParseExact(tokeni[5], "MM/dd/yyyy", null), tokeni[6], manifestacijeProdavca, bool.Parse(tokeni[7]));
                if(k.IsIzbrisan != true)
                korisnici.Add(k);

            }
            }

            sr.Close();
            stream.Close();
            return korisnici;
        }

        public static List<Manifestacija> IscitajManifestacije(string putanja)
        {
            MestoOdrzavanja mO = new MestoOdrzavanja();
            Manifestacija m = new Manifestacija();
            List<Manifestacija> manifestacije = new List<Manifestacija>();
            putanja = HostingEnvironment.MapPath(putanja);
            FileStream stream = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string red = "";
            while ((red = sr.ReadLine()) != null)
            {
                string[] tokeni = red.Split(';');

                try
                {
                    mO = new MestoOdrzavanja(tokeni[5], double.Parse(tokeni[6]), tokeni[7], tokeni[8], bool.Parse(tokeni[9])); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    m = new Manifestacija(tokeni[0], tokeni[1], int.Parse(tokeni[2]), DateTime.ParseExact(tokeni[3], "MM/dd/yyyy HH:mm", null), 
                    double.Parse(tokeni[4]), mO, bool.Parse(tokeni[9]), bool.Parse(tokeni[10]), tokeni[11]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (m.IsIzbrisana != true)
                    manifestacije.Add(m);
            }
          
            sr.Close();
            stream.Close();
            return manifestacije;
        }

        public static Dictionary<string, Karta> IscitajKarte(string putanja)
        {
            List<Manifestacija> manifestacije = IscitajManifestacije("~/App_Data/manifestacije.txt");
            Manifestacija man = new Manifestacija();

            Dictionary<string, Karta> karte = new Dictionary<string, Karta>();
            putanja = HostingEnvironment.MapPath(putanja);
            FileStream stream = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string red = "";
            while ((red = sr.ReadLine()) != null)
            {
                string[] tokeni = red.Split(';');

                foreach (var m in manifestacije)
                {
                    if (tokeni[1] == m.Naziv)
                    {
                        man = m;
                    }
                }
                Karta k = new Karta(tokeni[0], man, DateTime.ParseExact(tokeni[2], "MM/dd/yyyy HH:mm", null), double.Parse(tokeni[3]), tokeni[4], bool.Parse(tokeni[5]), (TipKarte)Enum.Parse(typeof(TipKarte), tokeni[6]), bool.Parse(tokeni[7]));
                if (!k.IsIzbrisana)
                    karte.Add(k.ID, k);
            }

            sr.Close();
            stream.Close();
            return karte;
        }

        public static List<Komentar> IscitajKomentare(string putanja)
        {
            List<Komentar> komentari = new List<Komentar>();
            putanja = HostingEnvironment.MapPath(putanja);
            FileStream stream = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(stream);
            string red = "";
            while ((red = sr.ReadLine()) != null)
            {
                string[] tokeni = red.Split(';');
                Komentar k = new Komentar(double.Parse(tokeni[0]), tokeni[1], tokeni[2], tokeni[3], Int32.Parse(tokeni[4]), bool.Parse(tokeni[5]), bool.Parse(tokeni[6]));
                komentari.Add(k);
            }

            sr.Close();
            stream.Close();
            return komentari;
        }

        public static void UpisiKorisnika(Korisnik korisnik)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/korisnici.txt");
            FileStream stream = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(stream);

            #region datum i vreme
            string mesec = korisnik.DatumRodjenja?.Month.ToString();
            string dan = korisnik.DatumRodjenja?.Day.ToString();
            string godina = korisnik.DatumRodjenja?.Year.ToString();
            if (korisnik.DatumRodjenja?.Month <= 9)
            {
                mesec = '0' + mesec;
            }
            if (korisnik.DatumRodjenja?.Day <= 9)
            {
                dan = '0' + dan;
            }
            if (korisnik.DatumRodjenja?.Year < 1000 && korisnik.DatumRodjenja?.Year > 99)
            {
                godina = '0' + godina;
            }
            if (korisnik.DatumRodjenja?.Year < 100 && korisnik.DatumRodjenja?.Year > 9)
            {
                godina = "00" + godina;
            }
            if (korisnik.DatumRodjenja?.Year < 10)
            {
                godina = "000" + godina;
            }
            #endregion

            string objectToWrite = korisnik.KorisnickoIme + ";" + korisnik.Lozinka + ";" + korisnik.Ime + ";" + korisnik.Prezime + ";"
            + korisnik.Pol + ";" + mesec + "/" + dan + "/"  + godina + ";" + korisnik.Uloga + ";" + "false";

            if (korisnik.Uloga == "kupac")
            {
                objectToWrite = objectToWrite + ";" + " , " + ";" + "0" + ";" + "bronzani";
            }

            if (korisnik.Uloga == "prodavac")
            {
                objectToWrite = objectToWrite + ";" + " , ";
            }
            
            sw.WriteLine(objectToWrite);

            sw.Close();
            stream.Close();
        }

        public static void UpisiManifestaciju(Manifestacija manifestacija)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/manifestacije.txt");


            FileStream stream = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(stream);


            string mesec = manifestacija.DatumIVremeOdrzavanja?.Month.ToString();
            string dan = manifestacija.DatumIVremeOdrzavanja?.Day.ToString();
            string godina = manifestacija.DatumIVremeOdrzavanja?.Year.ToString();
            string sati = manifestacija.DatumIVremeOdrzavanja?.Hour.ToString();
            string minuta = manifestacija.DatumIVremeOdrzavanja?.Minute.ToString();

            string objectToWrite = manifestacija.Naziv + ";" + manifestacija.TipManifestacije + ";" + manifestacija.BrojMesta.ToString() + ";" + mesec + "/" + dan + "/" + godina + " " + sati + ":" + minuta + ";"
            + manifestacija.CenaRegularneKarte + ";" + manifestacija.MestoOdrzavanja.Mesto + ";" + manifestacija.MestoOdrzavanja.PostanskiBroj.ToString() + ";" + manifestacija.MestoOdrzavanja.Ulica + ";"
            + manifestacija.MestoOdrzavanja.Broj + ";" + "false" + ";" + "false" + ";" + manifestacija.Naziv + ".jpg";

            sw.WriteLine(objectToWrite);

            sw.Close();
            stream.Close();
        }

        public static void UpisiKomentar(Komentar komentar)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/komentari.txt");
            FileStream stream = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(stream);

            string objectToWrite = komentar.IdKomentara.ToString() + ";" + komentar.Kupac + ";" + komentar.Manifestacija + ";" + komentar.Tekst + ";" + komentar.Ocena.ToString() + ";" + komentar.IsOdobren.ToString().ToLower() +
                ";" + komentar.IsIzbrisan.ToString().ToLower();
            sw.WriteLine(objectToWrite);

            sw.Close();
            stream.Close();
        }

        public static void IzmeniKomentar(Komentar komentar)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/komentari.txt");
            int count = 0;
            int index = -1;

            string[] komentari = File.ReadAllLines(path);

            foreach (var item in komentari)
            {
                count++;
                string[] tokeni = item.Split(';');
                if (komentar.IdKomentara == double.Parse(tokeni[0]))
                {
                    index = count;
                    break;
                }
            }

            string izmenjenKomentar = komentar.IdKomentara.ToString() + ";" + komentar.Kupac + ";" + komentar.Manifestacija + ";" + komentar.Tekst + ";" + 
            komentar.Ocena.ToString() + ";" + komentar.IsOdobren.ToString().ToLower() + ";" + komentar.IsIzbrisan.ToString().ToLower();

            komentari[index - 1] = izmenjenKomentar;
            File.WriteAllLines(path, komentari);
        }

        public static void IzmeniKorisnika(Korisnik korisnik)
        {
            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"~/App_Data/korisnici.txt");

            string putanja = HostingEnvironment.MapPath("~/App_Data/korisnici.txt");
            string[] korisnici = File.ReadAllLines(putanja);
            int count = 0;
            int index = -1;
            int brojElemenata = 0;

            string manifestacijeProdavca = "";
            string poeniKupca = "";
            string tipKupca = "";
            string karteKupca = "";


            foreach (var item in korisnici)
            {
                count++;
                string[] tokeni = item.Split(';');
                
                if (tokeni[0] == korisnik.KorisnickoIme)
                {
                    brojElemenata = tokeni.Count();
                    index = count;

                    if (brojElemenata == 9)
                    {
                        int brManifestacija = korisnik.Manifestacije.Count;
                        int brUcitanihManifestacija = tokeni[8].Split(',').Length;
                        if(brUcitanihManifestacija < brManifestacija)
                        {
                            string dodanaManifestacija = korisnik.Manifestacije[brManifestacija - 1].Naziv;
                            manifestacijeProdavca = tokeni[8] + "," + dodanaManifestacija;
                        }
                        else
                        {
                            manifestacijeProdavca = tokeni[8];
                        }
                        break;    
                    }
                    if (brojElemenata == 11)
                    {
                        poeniKupca = tokeni[9];
                        tipKupca = tokeni[10];
                        karteKupca = tokeni[8];
                        break;
                    }
                }
            }


            #region datum i vreme
            string mesec = korisnik.DatumRodjenja?.Month.ToString();
            string dan = korisnik.DatumRodjenja?.Day.ToString();
            string godina = korisnik.DatumRodjenja?.Year.ToString();
            if (korisnik.DatumRodjenja?.Month <= 9)
            {
                mesec = '0' + mesec;
            }
            if (korisnik.DatumRodjenja?.Day <= 9)
            {
                dan = '0' + dan;
            }
            if (korisnik.DatumRodjenja?.Year < 1000 && korisnik.DatumRodjenja?.Year > 99)
            {
                godina = '0' + godina;
            }
            if (korisnik.DatumRodjenja?.Year < 100 && korisnik.DatumRodjenja?.Year > 9)
            {
                godina = "00" + godina;
            }
            if (korisnik.DatumRodjenja?.Year < 10)
            {
                godina = "000" + godina;
            }
            #endregion

            string k = "";

            if (brojElemenata == 8)
            {
                k = korisnik.KorisnickoIme + ";" + korisnik.Lozinka + ";" + korisnik.Ime + ";" + korisnik.Prezime + ";"
                + korisnik.Pol + ";" + mesec + "/" + dan + "/" + godina + ";" + "administrator" + ";" + korisnik.IsIzbrisan.ToString().ToLower();
            }
            else if (brojElemenata == 11)
            {
                k = korisnik.KorisnickoIme + ";" + korisnik.Lozinka + ";" + korisnik.Ime + ";" + korisnik.Prezime + ";"
                + korisnik.Pol + ";" + mesec + "/" + dan + "/" + godina + ";" + "kupac" + ";" + korisnik.IsIzbrisan.ToString().ToLower() + ";" + karteKupca + ";" + poeniKupca + ";" + tipKupca;
            }
            else
            {
                k = korisnik.KorisnickoIme + ";" + korisnik.Lozinka + ";" + korisnik.Ime + ";" + korisnik.Prezime + ";"
                + korisnik.Pol + ";" + mesec + "/" + dan + "/" + godina + ";" + "prodavac" + ";" + korisnik.IsIzbrisan.ToString().ToLower() + ";" +  manifestacijeProdavca;
            }

            korisnici[index - 1] = k;
            File.WriteAllLines(putanja, korisnici);
        }

        public static void IzmeniManifestaciju(Manifestacija manifestacija)
        {
            string path = HostingEnvironment.MapPath("~/App_Data/manifestacije.txt");
            int count = 0;
            int index = -1;

            string[] manifestacije = File.ReadAllLines(path);

            foreach (var item in manifestacije)
            {
                count++;
                string[] tokeni = item.Split(';');
                if (manifestacija.Naziv == tokeni[0])
                {
                    index = count;
                    break;
                }
            }

            #region datum i vreme
            string mesec = manifestacija.DatumIVremeOdrzavanja?.Month.ToString();
            string dan = manifestacija.DatumIVremeOdrzavanja?.Day.ToString();
            string godina = manifestacija.DatumIVremeOdrzavanja?.Year.ToString();
            string sati = manifestacija.DatumIVremeOdrzavanja?.Hour.ToString();
            string minuta = manifestacija.DatumIVremeOdrzavanja?.Minute.ToString();
            if (manifestacija.DatumIVremeOdrzavanja?.Month <= 9)
            {
                mesec = '0' + mesec;
            }
            if (manifestacija.DatumIVremeOdrzavanja?.Day <= 9)
            {
                dan = '0' + dan;
            }
            if (manifestacija.DatumIVremeOdrzavanja?.Year < 1000 && manifestacija.DatumIVremeOdrzavanja?.Year > 99)
            {
                godina = '0' + godina;
            }
            if (manifestacija.DatumIVremeOdrzavanja?.Year < 100 && manifestacija.DatumIVremeOdrzavanja?.Year > 9)
            {
                godina = "00" + godina;
            }
            if (manifestacija.DatumIVremeOdrzavanja?.Year < 10)
            {
                godina = "000" + godina;
            }
            if(manifestacija.DatumIVremeOdrzavanja?.Hour < 10)
            {
                sati = "0" + sati;
            }
            if (manifestacija.DatumIVremeOdrzavanja?.Minute < 10)
            {
                minuta = "0" + minuta;
            }

            #endregion

            string izmenaManifestacije = manifestacija.Naziv + ";" + manifestacija.TipManifestacije + ";" + manifestacija.BrojMesta.ToString() + ";" + mesec + "/" + dan + "/" + godina + " " + sati + ":" + minuta  + ";"
                 + manifestacija.CenaRegularneKarte + ";" + manifestacija.MestoOdrzavanja.Mesto + ";" + manifestacija.MestoOdrzavanja.PostanskiBroj.ToString() + ";"
                 + manifestacija.MestoOdrzavanja.Ulica + ";" + manifestacija.MestoOdrzavanja.Broj + ";" + manifestacija.IsAktivna.ToString().ToLower() + ";" + manifestacija.IsIzbrisana.ToString().ToLower() + ";"
                 + manifestacija.Poster;

            manifestacije[index - 1] = izmenaManifestacije;
            File.WriteAllLines(path, manifestacije);
        }

    }

}