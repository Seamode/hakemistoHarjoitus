using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

namespace hakemistoHarjoitus
{

    class Program
    {
        static String outDire_g = null;
        static String outFile_g = null;
        static ArrayList rivit_g = null;
        static int laskuri_g = 0;
        static void Main(string[] args)
        {
            
            String esimerkki = "SeaMODE_20190928_112953.csv";
            Console.WriteLine("Esimerkin pituus ==> " + esimerkki.Length);
            int pit = esimerkki.Length;
            List<String> filekset = new List<String>();
            String startPattern = "^SeaMODE_20190928_";
            DirectoryInfo di = new DirectoryInfo(@"D:\scrum projekti\myairbridge-kSWEva1J3");
           
            foreach (var fi in di.GetFiles())
            {
                // Tähän regexp
                if (Regex.IsMatch(fi.Name, startPattern) && Regex.IsMatch(fi.Name, ".csv$") && fi.Name.Length == pit)
                    filekset.Add(fi.FullName);
                if (Program.outDire_g == null)
                    outDire_g = new String(fi.DirectoryName);
            }
            Console.WriteLine("Hakemisto on " + outDire_g);
            Console.WriteLine("Fileksiä oli ==> " + filekset.Count);
            teeWriteFile();
            rivit_g = new ArrayList();
            Boolean isEka = true;
           foreach (String fileName in filekset) {
                lueTiedosto(fileName, isEka);
                isEka = false;
            }
            if(rivit_g.Count > 0)
                kirjoita();
        }
        
       
        private static void lueTiedosto(String fileName, Boolean isEka)
        {
            String riviOtsikkoPattern = "^Date_PC;Time_PC";
           
            using (StreamReader sr = File.OpenText(fileName))
            {
                String luettu = "";
                Boolean isOtsikkoOhi = false;
                while ((luettu = sr.ReadLine()) != null)
                {
                    // Otsikko luette nyt kirjoitetaan tietueet. Vielä tarkistus täsmääkö kellonaika
                    if (isOtsikkoOhi)
                    {
                        rivit_g.Add(luettu);
                        laskuri_g++;
                    }

                    // Kirjoitetaan ensimmäiseltä luetulta tiedostola xml otsikko
                    if (isEka && !isOtsikkoOhi)
                    {
                        rivit_g.Add(luettu);
                        laskuri_g++;
                        if (Regex.IsMatch(luettu, riviOtsikkoPattern))
                            isOtsikkoOhi = true;
                        
                    }

                    // Tarkistetaan muilta kuin ensimmäiseltä tiedostolta onko otsikko jo ohi
                    if (!isEka && !isOtsikkoOhi)
                        if (Regex.IsMatch(luettu, riviOtsikkoPattern))
                            isOtsikkoOhi = true;
                }
            }

        }
        private static void kirjoita()
        {
           
            // Stream writer ei osaa kirjoittaa ArrayList tyyppisestä. Siksi kopioidaan
            string[] asArr = new string[rivit_g.Count];
            rivit_g.CopyTo(asArr);
            
            Console.WriteLine("Rivejä on ==> " + laskuri_g + " tai ==> " + rivit_g.Count);
            if (rivit_g.Count == 0)
                return;

           Console.WriteLine("Ruvetaan kirjoittamaan tiedostoon ==> " + outFile_g);
            //System.IO.File.WriteAllLines(outFile_g, rivit);

            using (StreamWriter sw = File.CreateText(outFile_g))
            {
                foreach (string line in asArr)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
            
        }
        private static void teeWriteFile()
            // Tätä ei ilmeisesti tarvitakkaan
        {
            outFile_g = outDire_g + "\\koonti.csv";
            /*if (!File.Exists(outFile_g))
              File.CreateText(outFile_g);*/
          
        }
    }
}
