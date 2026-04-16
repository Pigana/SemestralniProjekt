using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Semestralni_Projekt
{
    internal class Program
    {
        //třída pro reprezentaci zboží, obsahuje název, cenu, ID a počet kusů
        class Zbozi
        {
            public string nazev;
            public double cena;
            public int ID;
            public int pocet;
        }
        //funkce pro uložení seznamu produktů do souboru
        static void Ulozit(List<Zbozi> zboziList)
        {
            using (StreamWriter sw = new StreamWriter(@"soubor.csv"))
            {
                foreach (var z in zboziList)
                {
                    sw.WriteLine($"{z.ID};{z.nazev};{z.cena}");
                }
                sw.Flush();
            }
        }
        //funkce pro načtení seznamu produktů ze souboru
        static void Nacist(List<Zbozi> zboziList)
        {
            if (File.Exists("soubor.csv"))
            {
                zboziList.Clear();//vyčistí se seznam, aby nedošlo k duplikaci produktů při načítán
                using (StreamReader sr = new StreamReader("soubor.csv"))
                {
                    string radek;
                    string[] pole = new string[3];
                    while (!(sr.EndOfStream))
                    {
                        radek = sr.ReadLine();
                        pole = radek.Split(';');
                        zboziList.Add(new Zbozi { ID = int.Parse(pole[0]), nazev = pole[1], cena = double.Parse(pole[2]) });
                    }

                }
                Console.WriteLine("Načteno");
            }
            else
            {
                Console.WriteLine("Soubor neexistuje.");
            }

        }
        //funkce pro smazání produktu ze seznamu podle zadaného ID
        static void VymazatProdukt(List<Zbozi> zboziList)
        {
            int idSmazat;
            zboziList.ForEach(z => Console.WriteLine(z.ID + "   " + z.nazev));//výpis ID a názvu produktů pro snadnější orientaci při mazání
            Console.WriteLine("------------------------");
            Console.Write("Zadejte ID produktu, který chcete smazat: ");
            while (!int.TryParse(Console.ReadLine(), out idSmazat) || !zboziList.Any(z => z.ID == idSmazat))
            {
                Console.Write("Zadejte platné ID: ");
            }
            zboziList.RemoveAll(z => z.ID == idSmazat);
            Console.WriteLine("------------------------");
            Console.WriteLine($"Produkt s ID {idSmazat} byl úspěšně odstraněn.");
        }
        //funkce pro markování zboží, výpočet celkové ceny a výpis účtenky
        static void Markovat(List<Zbozi> zboziList)
        {
            int kod, mnozstvi;
            double celkovaCena = 0;
            char odpoved;
            do
            {
                Console.Clear();
                Console.WriteLine("ID  Název");
                zboziList.ForEach(z => Console.WriteLine($"{z.ID,-3} {z.nazev}"));

                Console.WriteLine("------------------------");
                Console.WriteLine("Celková cena:" + celkovaCena + " Kč");
                Console.WriteLine("------------------------");
                Console.Write("Zadejte ID zboží, které chcete markovat: ");
                while (!int.TryParse(Console.ReadLine(), out kod) || !zboziList.Any(z => z.ID == kod))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Zadané ID musí být číslo a musí být v seznamu!");
                    Console.ResetColor();
                    Console.Write("Zkuste to znovu: ");

                }
                //hledáme zboží podle zadaného ID
                Zbozi hledane = zboziList.Find(z => z.ID == kod);

                Console.Write("Zadejte množství: ");
                while (!int.TryParse(Console.ReadLine(), out mnozstvi))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Zadané množství musí být celé číslo!");
                    Console.ResetColor();
                    Console.Write("Zkuste to znovu: ");
                }
                hledane.pocet += mnozstvi;
                celkovaCena += (mnozstvi * hledane.cena);
                Console.Write("Stiskněte libovolnou klávesu pro pokračování, 't' pro ukončení.");

                odpoved = char.ToLower(Console.ReadKey().KeyChar);
            } while (odpoved != 't');


            //Výpis účtenky
            Console.Clear();
            Console.WriteLine($"{"Účet",13}");
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("------------------------");
            foreach (var z in zboziList)
            {
                if (z.pocet > 0)
                {
                    Console.WriteLine($"{z.pocet,-2} {z.nazev,-10}   {Math.Round(z.cena * z.pocet, 1),5} Kč");
                }
            }
            Console.WriteLine("------------------------");
            celkovaCena = Math.Round(celkovaCena, 2);
            Console.WriteLine($"CELKEM {celkovaCena,14} Kč");

            Console.ReadKey();
            //reset množství pro další markování
            zboziList.ForEach(z => z.pocet = 0);

        }
        //Funkce pro přidání produktu do seznamu
        static void Pridat(List<Zbozi> zboziList)
        {
            string nazev;
            int zadaneID;
            Zbozi produkt = new Zbozi();
            Console.Write("Zadejte název zboží: ");
            nazev = Console.ReadLine();
            while (nazev.Length < 2)
            {
                Console.Write("Zadejte platný název: ");
                nazev = Console.ReadLine();
            }
            produkt.nazev = nazev;
            Console.Write("Zadejte cenu zboží: ");
            while (!double.TryParse(Console.ReadLine(), out produkt.cena))
            {
                Console.Write("Zadejte platnou cenu: ");
            }
            Console.Write("Zadejte ID zboží: ");
            while (!int.TryParse(Console.ReadLine(), out zadaneID) || zboziList.Any(z => z.ID == zadaneID))//kontrola, zda je ID celé číslo a zda již není použito
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ID musí být celé číslo a nesmí být již použito!");
                Console.ResetColor();
                Console.Write("Zadejte jiné ID: ");
            }
            produkt.ID = zadaneID;
            produkt.pocet = 0;
            zboziList.Add(produkt);
        }


        static void Main(string[] args)
        {
            //seznam produktů
            List<Zbozi> zboziList = new List<Zbozi>();

            char odpoved;

            //MENU
            do
            {
                Console.Clear();
                Console.WriteLine("Pokladna - MENU");
                Console.WriteLine("------------------------");
                Console.WriteLine("Přidat produkt [p]");
                Console.WriteLine("Vymazat produkt [u]");
                Console.WriteLine("Vypsat produkty [v]");
                Console.WriteLine("Vymazat produkty [w]");
                Console.WriteLine("Ulozit do souboru [b]");
                Console.WriteLine("Nacist ze souboru [n]");
                Console.WriteLine("Markovat [m]");
                Console.WriteLine("Konec [k]");
                Console.Write("Zadejte akci: ");
                odpoved = char.ToLower(Console.ReadKey().KeyChar);
                switch (odpoved)
                {
                    case 'p':
                        Console.Clear();
                        Pridat(zboziList);
                        break;
                    case 'm':
                        Console.Clear();
                        Console.WriteLine("Pokladna - Markovat");
                        Console.WriteLine("------------------------");
                        if (zboziList.Count == 0)
                        {
                            Console.WriteLine("Seznam produktů je prázdný. Nejprve přidejte produkty.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Markovat(zboziList);
                        }
                        break;
                    case 'u':
                        Console.Clear();
                        Console.WriteLine("Pokladna - Vymazat produkt");
                        VymazatProdukt(zboziList);
                        Console.ReadKey();
                        break;
                    case 'w':
                        Console.Clear();
                        zboziList.Clear();
                        Console.WriteLine("Seznam produktů byl vymazán.");
                        Console.ReadKey();
                        break;
                    case 'v':
                        Console.Clear();
                        Console.WriteLine("Pokladna - Seznam produktů ");
                        Console.WriteLine("------------------------");
                        foreach (var zbozi in zboziList)
                        {
                            Console.WriteLine($"ID: {zbozi.ID}, Název: {zbozi.nazev}, Cena: {zbozi.cena} Kč");
                        }
                        Console.ReadKey();
                        break;
                    case 'b':
                        Console.Clear();
                        Ulozit(zboziList);
                        Console.WriteLine("Seznam produktů byl uložen do souboru.");
                        Console.ReadKey();
                        break;
                    case 'n':
                        Console.Clear();
                        Nacist(zboziList);
                        Console.ReadKey();
                        break;
                    case 'k':
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Neplatná akce.");
                        Console.ReadKey();
                        break;
                }
            } while (odpoved != 'k');

        }
    }

}