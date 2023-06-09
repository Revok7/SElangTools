﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

using System.Diagnostics;
using System.ComponentModel;

using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;

using Newtonsoft.Json;
using PWRlangConverter;

namespace SElangTools
{

    class SElangTools
    {


        readonly static string _SElangTools_naglowek = "SElangTools v.1.06 by Revok (2023)";

        const string skrypt = "SElangTools.cs";
        static public string folderglownyprogramu = Directory.GetCurrentDirectory();

        static DateTime aktualny_czas = DateTime.Now;


        public class RekordJSON : IEquatable<RekordJSON>, IComparable<RekordJSON>
        //klasa dla danych wczytywanych z plików .json zawierających lokalizację gry
        {
            public int ID { get; set; }

            public string Plik { get; set; }
            public string Klucz { get; set; }
            public string String { get; set; }

            public override string ToString()
            {
                return "ID: " + ID + "\n" + "Plik: " + Plik + "\n" + "Klucz: " + Klucz + "\n" + "String: " + String;
            }
            public override bool Equals(object obiekt)
            {
                if (obiekt == null) return false;
                RekordJSON obiektrekordu = obiekt as RekordJSON;
                if (obiektrekordu == null) return false;
                else return Equals(obiektrekordu);
            }

            /*
            public int SortujRosnacoWedlugNazwy(string nazwa1, string nazwa2)
            {

                return nazwa1.CompareTo(nazwa2);
            }
            */

            // Domyślny komparator dla typu Rekord.
            public int CompareTo(RekordJSON porownaniezRekordem)
            {
                // Wartość null oznacza, że ten obiekt jest większy.
                if (porownaniezRekordem == null)
                    return 1;

                else
                    return this.ID.CompareTo(porownaniezRekordem.ID);
            }

            public override int GetHashCode()
            {
                return ID;
            }
            public bool Equals(RekordJSON other)
            {
                if (other == null) return false;
                return (this.ID.Equals(other.ID));
            }
            // Powinien również nadpisać operatory == i !=.
        }



        private static void Blad(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        private static void Sukces2(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }
        private static void Informacja(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }

        private static string PoliczPostepWProcentach(double aktualna_linia, double wszystkich_linii)
        {
            double rezultat = (aktualna_linia / wszystkich_linii) * 100;

            return Math.Round(rezultat, 0).ToString();
        }

        private static string PobierzTimestamp(DateTime wartosc)
        {
            return wartosc.ToString("yyyyMMddHHmmss");
        }

        private static bool CzyParsowanieINTUdane(string wartosc)
        {
            bool rezultat_bool = false;
            int rezultat_int = -1;

            if (int.TryParse(wartosc, out rezultat_int))
            {
                rezultat_bool = true;
            }

            return rezultat_bool;
        }

        public static uint PoliczLiczbeLinii(string nazwa_pliku)
        {
            uint liczbalinii = 0;

            if (File.Exists(nazwa_pliku))
            {
                FileStream plik_fs = new FileStream(nazwa_pliku, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plik_sr = new StreamReader(plik_fs);

                    while (plik_sr.Peek() != -1)
                    {
                        plik_sr.ReadLine();
                        liczbalinii++;
                    }

                    plik_sr.Close();

                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do pliku (metoda: PoliczLiczbeLinii).");
                    Console.ResetColor();
                }

                plik_fs.Close();

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Nie istnieje wskazany plik (metoda: PoliczLiczbeLinii).");
                Console.ResetColor();
            }

            return liczbalinii;
        }

        public static bool CzyIstniejeDanyKluczWLiscieKluczy(List<dynamic> lista_kluczy, string szukany_dany_klucz)
        {
            bool rezultat = false;

            rezultat = lista_kluczy.Exists(x => x == szukany_dany_klucz);

            return rezultat;
        }

        public static int PobierzNumerIndeksuZListyKluczyIStringow(dynamic[] tablica_list_kluczy_i_stringow, string szukany_dany_klucz)
        {
            int znaleziony_index = -1;

            if (tablica_list_kluczy_i_stringow.Length == 2)
            {
                List<dynamic> lista_kluczy = tablica_list_kluczy_i_stringow[0];
                List<List<dynamic>> lista_stringow = tablica_list_kluczy_i_stringow[1];

                znaleziony_index = lista_kluczy.IndexOf(szukany_dany_klucz);

            }

            return znaleziony_index;
        }

        public static List<string> PobierzNazwyPlikowJSONzFolderu(string nazwa_folderu)
        {
            List<string> nazwy_plikow_JSON = new List<string>();

            string folderglowny = Directory.GetCurrentDirectory();

            if (Directory.Exists(folderglowny + "//" + nazwa_folderu) == true)
            {
                string[] plikiJSONwfolderze_nazwy = Directory.GetFiles(folderglowny + "//" + nazwa_folderu, "*.json");

                foreach (string s in plikiJSONwfolderze_nazwy)
                {
                    FileInfo plik_fileinfo = null;

                    try
                    {
                        plik_fileinfo = new FileInfo(s);

                        nazwy_plikow_JSON.Add(plik_fileinfo.Name);
                    }
                    catch (FileNotFoundException e)
                    {
                        Blad("Blad: " + e.Message);
                        continue;
                    }

                }

            }
            else
            {
                Blad("Blad: Nie istnieje folder o nazwie: " + nazwa_folderu);
            }

            return nazwy_plikow_JSON;


        }

        static void Main(string[] args)
        {
            Console.Title = _SElangTools_naglowek;

            Process[] procesy_nazwa = Process.GetProcessesByName("SElangTools");

            if (procesy_nazwa.Length <= 1)
            {

                string numer_operacji_string;

                Console.WriteLine(_SElangTools_naglowek);

                Console.WriteLine("WAŻNE: Pliki poddawane operacjom muszą zostać skopiowane wcześniej do folderu z tym programem.");
                Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("UWAGA!: W celu konwertowania plików pochodzących z platformy Transifex oraz wdrażania aktualizacji (w formacie JSON) należy używać narzędzi PWRlangTools oraz PWRlangConverter z metadanymi dla gry Space Engineers."); Console.ResetColor();
                Console.WriteLine("---[SpaceEngineers_PL]:");
                Console.WriteLine("1. [JSON->RESX] Konwersja pliku lokalizacyjnego JSON do RESX. ");
                Console.WriteLine(" ");
                Console.WriteLine("2. [RESX->KeyValueJSON->JSON] Konwersja pliku lokalizacyjnego RESX do JSON.");
                Console.WriteLine("3. [1xJSON->2xTransifex.com.TXT] Konwersja pliku JSON do plików TXT przeznaczonych dla platformy Transifex.com (z identyfikatorami numerów linii według pliku JSON).");
                Console.WriteLine(" ");
                Console.WriteLine("4. [2xJSON->JSON] Przeniesienie treści stringów ze źródłowego pliku JSON według szablonu do nowego pliku JSON.");
                Console.WriteLine("---------------------------------------");
                Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");
                numer_operacji_string = Console.ReadLine();



                if (CzyParsowanieINTUdane(numer_operacji_string))
                {
                    int numer_operacji_int = int.Parse(numer_operacji_string);

                    if (numer_operacji_int == 1)
                    {
                        JSONtoRESX();
                    }
                    else if (numer_operacji_int == 2)
                    {
                        RESXtoJSON();
                    }
                    else if (numer_operacji_int == 3)
                    {
                        JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON_v2();
                    }
                    else if (numer_operacji_int == 4)
                    {
                        JSONplusJSONtoJSON_PrzeniesienieStringowWedlugSzablonu_v2();
                    }

                    else
                    {
                        Blad("Podano błędny numer operacji.");

                    }

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();

                }
                else
                {
                    Blad("Podano błedny numer operacji.");

                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }

            }
            else
            {
                Blad("Aplikacja SElangTools jest aktualnie uruchomiona w innym oknie lub nazwa pliku wykonywalnego jest nieprawidłowa.");

                Console.WriteLine("Kliknij ENTER aby zamknąć to okno.");
                Console.ReadKey();
            }

        }


        private static bool UtworzNaglowekJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            if (nazwafolderu == "")
            {
                plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }
            else
            {
                plikJSON_fs = new FileStream(nazwafolderu + "//" + nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }

            try
            {
                StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                plikJSON_sw.WriteLine("{");
                plikJSON_sw.WriteLine("  \"$id\": \"1\",");
                plikJSON_sw.WriteLine("  \"strings\": {");

                plikJSON_sw.Close();

                rezultat = true;

            }
            catch
            {
                rezultat = false;
            }

            plikJSON_fs.Close();


            return rezultat;
        }

        private static bool UtworzStopkeJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            string sprawdzenie_istnienia;

            if (nazwafolderu == "")
            {
                sprawdzenie_istnienia = nazwaplikuJSON;
            }
            else
            {
                sprawdzenie_istnienia = nazwafolderu + "//" + nazwaplikuJSON;
            }

            if (File.Exists(sprawdzenie_istnienia))
            {
                if (nazwafolderu == "")
                {
                    plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    plikJSON_fs = new FileStream(nazwafolderu + "//" + nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }

                try
                {
                    StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                    plikJSON_sw.WriteLine("  }");
                    plikJSON_sw.Write("}");

                    plikJSON_sw.Close();

                    rezultat = true;

                }
                catch
                {
                    rezultat = false;
                }

                plikJSON_fs.Close();

            }
            else
            {
                rezultat = false;
            }

            return rezultat;
        }

        private static bool UtworzNaglowekRESX(string nazwaplikuRESX, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikRESX_fs;

            if (nazwafolderu == "")
            {
                plikRESX_fs = new FileStream(nazwaplikuRESX, FileMode.Create, FileAccess.Write);
            }
            else
            {
                plikRESX_fs = new FileStream(nazwafolderu + "//" + nazwaplikuRESX, FileMode.Create, FileAccess.Write);
            }

            try
            {
                StreamWriter plikRESX_sw = new StreamWriter(plikRESX_fs);

                plikRESX_sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                plikRESX_sw.WriteLine("<root>");
                plikRESX_sw.WriteLine("  <!-- ");
                plikRESX_sw.WriteLine("    Microsoft ResX Schema ");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    Version 2.0");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    The primary goals of this format is to allow a simple XML format ");
                plikRESX_sw.WriteLine("    that is mostly human readable. The generation and parsing of the ");
                plikRESX_sw.WriteLine("    various data types are done through the TypeConverter classes ");
                plikRESX_sw.WriteLine("    associated with the data types.");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    Example:");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    ... ado.net/XML headers & schema ...");
                plikRESX_sw.WriteLine("    <resheader name=\"resmimetype\">text/microsoft-resx</resheader>");
                plikRESX_sw.WriteLine("    <resheader name=\"version\">2.0</resheader>");
                plikRESX_sw.WriteLine("    <resheader name=\"reader\">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>");
                plikRESX_sw.WriteLine("    <resheader name=\"writer\">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>");
                plikRESX_sw.WriteLine("    <data name=\"Name1\"><value>this is my long string</value><comment>this is a comment</comment></data>");
                plikRESX_sw.WriteLine("    <data name=\"Color1\" type=\"System.Drawing.Color, System.Drawing\">Blue</data>");
                plikRESX_sw.WriteLine("    <data name=\"Bitmap1\" mimetype=\"application/x-microsoft.net.object.binary.base64\">");
                plikRESX_sw.WriteLine("        <value>[base64 mime encoded serialized .NET Framework object]</value>");
                plikRESX_sw.WriteLine("    </data>");
                plikRESX_sw.WriteLine("    <data name=\"Icon1\" type=\"System.Drawing.Icon, System.Drawing\" mimetype=\"application/x-microsoft.net.object.bytearray.base64\">");
                plikRESX_sw.WriteLine("        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>");
                plikRESX_sw.WriteLine("        <comment>This is a comment</comment>");
                plikRESX_sw.WriteLine("    </data>");
                plikRESX_sw.WriteLine("                ");
                plikRESX_sw.WriteLine("    There are any number of \"resheader\" rows that contain simple ");
                plikRESX_sw.WriteLine("    name/value pairs.");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    Each data row contains a name, and value. The row also contains a ");
                plikRESX_sw.WriteLine("    type or mimetype. Type corresponds to a .NET class that support ");
                plikRESX_sw.WriteLine("    text/value conversion through the TypeConverter architecture. ");
                plikRESX_sw.WriteLine("    Classes that don't support this are serialized and stored with the ");
                plikRESX_sw.WriteLine("    mimetype set.");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    The mimetype is used for serialized objects, and tells the ");
                plikRESX_sw.WriteLine("    ResXResourceReader how to depersist the object. This is currently not ");
                plikRESX_sw.WriteLine("    extensible. For a given mimetype the value must be set accordingly:");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    Note - application/x-microsoft.net.object.binary.base64 is the format ");
                plikRESX_sw.WriteLine("    that the ResXResourceWriter will generate, however the reader can ");
                plikRESX_sw.WriteLine("    read any of the formats listed below.");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    mimetype: application/x-microsoft.net.object.binary.base64");
                plikRESX_sw.WriteLine("    value   : The object must be serialized with ");
                plikRESX_sw.WriteLine("            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter");
                plikRESX_sw.WriteLine("            : and then encoded with base64 encoding.");
                plikRESX_sw.WriteLine("    ");
                plikRESX_sw.WriteLine("    mimetype: application/x-microsoft.net.object.soap.base64");
                plikRESX_sw.WriteLine("    value   : The object must be serialized with ");
                plikRESX_sw.WriteLine("            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter");
                plikRESX_sw.WriteLine("            : and then encoded with base64 encoding.");
                plikRESX_sw.WriteLine("");
                plikRESX_sw.WriteLine("    mimetype: application/x-microsoft.net.object.bytearray.base64");
                plikRESX_sw.WriteLine("    value   : The object must be serialized into a byte array ");
                plikRESX_sw.WriteLine("            : using a System.ComponentModel.TypeConverter");
                plikRESX_sw.WriteLine("            : and then encoded with base64 encoding.");
                plikRESX_sw.WriteLine("    -->");
                plikRESX_sw.WriteLine("  <xsd:schema id=\"root\" xmlns=\"\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">");
                plikRESX_sw.WriteLine("    <xsd:import namespace=\"http://www.w3.org/XML/1998/namespace\" />");
                plikRESX_sw.WriteLine("    <xsd:element name=\"root\" msdata:IsDataSet=\"true\">");
                plikRESX_sw.WriteLine("      <xsd:complexType>");
                plikRESX_sw.WriteLine("        <xsd:choice maxOccurs=\"unbounded\">");
                plikRESX_sw.WriteLine("          <xsd:element name=\"metadata\">");
                plikRESX_sw.WriteLine("            <xsd:complexType>");
                plikRESX_sw.WriteLine("              <xsd:sequence>");
                plikRESX_sw.WriteLine("                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" />");
                plikRESX_sw.WriteLine("              </xsd:sequence>");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"name\" use=\"required\" type=\"xsd:string\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"type\" type=\"xsd:string\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"mimetype\" type=\"xsd:string\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute ref=\"xml:space\" />");
                plikRESX_sw.WriteLine("            </xsd:complexType>");
                plikRESX_sw.WriteLine("          </xsd:element>");
                plikRESX_sw.WriteLine("          <xsd:element name=\"assembly\">");
                plikRESX_sw.WriteLine("            <xsd:complexType>");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"alias\" type=\"xsd:string\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"name\" type=\"xsd:string\" />");
                plikRESX_sw.WriteLine("            </xsd:complexType>");
                plikRESX_sw.WriteLine("          </xsd:element>");
                plikRESX_sw.WriteLine("          <xsd:element name=\"data\">");
                plikRESX_sw.WriteLine("            <xsd:complexType>");
                plikRESX_sw.WriteLine("              <xsd:sequence>");
                plikRESX_sw.WriteLine("                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />");
                plikRESX_sw.WriteLine("                <xsd:element name=\"comment\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"2\" />");
                plikRESX_sw.WriteLine("              </xsd:sequence>");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" msdata:Ordinal=\"1\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"type\" type=\"xsd:string\" msdata:Ordinal=\"3\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"mimetype\" type=\"xsd:string\" msdata:Ordinal=\"4\" />");
                plikRESX_sw.WriteLine("              <xsd:attribute ref=\"xml:space\" />");
                plikRESX_sw.WriteLine("            </xsd:complexType>");
                plikRESX_sw.WriteLine("          </xsd:element>");
                plikRESX_sw.WriteLine("          <xsd:element name=\"resheader\">");
                plikRESX_sw.WriteLine("            <xsd:complexType>");
                plikRESX_sw.WriteLine("              <xsd:sequence>");
                plikRESX_sw.WriteLine("                <xsd:element name=\"value\" type=\"xsd:string\" minOccurs=\"0\" msdata:Ordinal=\"1\" />");
                plikRESX_sw.WriteLine("              </xsd:sequence>");
                plikRESX_sw.WriteLine("              <xsd:attribute name=\"name\" type=\"xsd:string\" use=\"required\" />");
                plikRESX_sw.WriteLine("            </xsd:complexType>");
                plikRESX_sw.WriteLine("          </xsd:element>");
                plikRESX_sw.WriteLine("        </xsd:choice>");
                plikRESX_sw.WriteLine("      </xsd:complexType>");
                plikRESX_sw.WriteLine("    </xsd:element>");
                plikRESX_sw.WriteLine("  </xsd:schema>");
                plikRESX_sw.WriteLine("  <resheader name=\"resmimetype\">");
                plikRESX_sw.WriteLine("    <value>text/microsoft-resx</value>");
                plikRESX_sw.WriteLine("  </resheader>");
                plikRESX_sw.WriteLine("  <resheader name=\"version\">");
                plikRESX_sw.WriteLine("    <value>2.0</value>");
                plikRESX_sw.WriteLine("  </resheader>");
                plikRESX_sw.WriteLine("  <resheader name=\"reader\">");
                plikRESX_sw.WriteLine("    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>");
                plikRESX_sw.WriteLine("  </resheader>");
                plikRESX_sw.WriteLine("  <resheader name=\"writer\">");
                plikRESX_sw.WriteLine("    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>");
                plikRESX_sw.WriteLine("  </resheader>");


                plikRESX_sw.Close();

                rezultat = true;

            }
            catch
            {
                rezultat = false;
            }

            plikRESX_fs.Close();


            return rezultat;
        }

        private static bool UtworzStopkeRESX(string nazwaplikuRESX, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikRESX_fs;

            string sprawdzenie_istnienia;

            if (nazwafolderu == "")
            {
                sprawdzenie_istnienia = nazwaplikuRESX;
            }
            else
            {
                sprawdzenie_istnienia = nazwafolderu + "//" + nazwaplikuRESX;
            }

            if (File.Exists(sprawdzenie_istnienia))
            {
                if (nazwafolderu == "")
                {
                    plikRESX_fs = new FileStream(nazwaplikuRESX, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    plikRESX_fs = new FileStream(nazwafolderu + "//" + nazwaplikuRESX, FileMode.Append, FileAccess.Write);
                }

                try
                {
                    StreamWriter plikRESX_sw = new StreamWriter(plikRESX_fs);

                    plikRESX_sw.WriteLine("</root>");

                    plikRESX_sw.Close();

                    rezultat = true;

                }
                catch
                {
                    rezultat = false;
                }

                plikRESX_fs.Close();

            }
            else
            {
                rezultat = false;
            }

            return rezultat;
        }

        public static string FiltrujString(string tresc_stringa)
        //stringi wczytane poprzez moduł JSON muszą zostać przefiltrowane przez zapisaniem w zmiennych/listach danych itd.
        {
            return tresc_stringa.Replace("\n", "\\n")
                   .Replace("\"", "\\\"")
                   .Replace("\t", "\\t")
                   .Replace("\\\\", "\\\\\\");

        }


        public static string FiltrujStringDoZapisuWRESX(string tresc_stringa)
        //stringi dla RESX muszą zostać przefiltrowane przez zapisaniem w plikach
        {
            return tresc_stringa
                   .Replace("&", "&amp;")
                   //.Replace("\n", "\\n")
                   .Replace(">", "&gt;")
                   .Replace("<", "&lt;")
                   ;

        }

        private static void RESXtoKeyValueJSON(string nazwaplikuRESX)
        {

            if (File.Exists(nazwaplikuRESX) == true)
            {
                var plik_RESX = File.ReadAllText(nazwaplikuRESX);

                var surowe_dane = new
                {
                    id = "1",
                    strings = XElement.Parse(plik_RESX)
                        .Elements("data")
                        .Select(el => new
                        {
                            Key = el.Attribute("name").Value,
                            Value = el.Element("value").Value
                        })

                };

                string tresc_keyvaluejson = JsonConvert.SerializeObject(surowe_dane, Newtonsoft.Json.Formatting.Indented)
                    .Replace("  \"id\": \"1\",", "  \"$id\": \"1\",")
                    .Replace("\\\\n", "\\n");

                //Console.WriteLine(tresc_keyvaluejson);

                string nazwanowegoplikuKeyValueJSON = nazwaplikuRESX.Replace(".resx", ".KeyValue.json");

                if (File.Exists(nazwanowegoplikuKeyValueJSON) == true) { File.Delete(nazwanowegoplikuKeyValueJSON); }
                FileStream nowyplikJSON_fs = new FileStream(nazwanowegoplikuKeyValueJSON, FileMode.CreateNew, FileAccess.Write);
                try
                {
                    StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);

                    nowyplikJSON_sw.Write(tresc_keyvaluejson);

                    nowyplikJSON_sw.Close();
                }
                catch
                {
                    Blad("BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do pliku.");
                }
                nowyplikJSON_fs.Close();


            }
            else
            {
                Blad("Nie istnieje plik o nazwie \"" + nazwaplikuRESX + "\".");
            }

        }

        private static List<RekordJSON> WczytajDaneZPlikuJSONdoListyRekordow(string nazwa_pliku_JSON)
        {
            List<RekordJSON> danezplikuJSON_listarekordow = new List<RekordJSON>();

            if (File.Exists(nazwa_pliku_JSON))
            {

                dynamic[] danezplikuJSON_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(nazwa_pliku_JSON);

                List<dynamic> danezplikuJSON_listakluczy = danezplikuJSON_tablicalistdanych[0];
                List<List<dynamic>> danezplikuJSON_listastringow = danezplikuJSON_tablicalistdanych[1];

                for (int i2b = 0; i2b < danezplikuJSON_listakluczy.Count(); i2b++)
                {

                    if (i2b != 0 && i2b != 1) //odfiltrowanie pierwszych dwóch rekordów zawierających słowa, wczytane z pliku JSON, takie jak: "$id", "string", "1"
                    {
                        for (int i2c = 0; i2c < danezplikuJSON_listastringow[i2b].Count(); i2c++)
                        {

                            int _ID = i2b + 2;
                            string _Plik = nazwa_pliku_JSON;
                            string _Klucz = danezplikuJSON_listakluczy[i2b];
                            string _String = FiltrujString(danezplikuJSON_listastringow[i2b][i2c]);

                            //Console.WriteLine("[DEBUG] " + _ID + "|" + _Plik + "|" + _Klucz + "|" + _String);

                            danezplikuJSON_listarekordow.Add(new RekordJSON { ID = _ID, Plik = _Plik, Klucz = _Klucz, String = _String });

                        }

                    }


                }

            }

            return danezplikuJSON_listarekordow;
        }



        public static void RESXtoJSON()
        {
            string nazwaplikuRESX;
            Console.Write("Podaj nazwę pliku RESX: ");
            nazwaplikuRESX = Console.ReadLine();
            if (nazwaplikuRESX == "") { nazwaplikuRESX = "MyTexts.pl-PL.resx"; }


            RESXtoKeyValueJSON(nazwaplikuRESX);


            string nazwaplikuKeyValueJSON;
            string nazwadocelowegoplikuJSON;

            nazwaplikuKeyValueJSON = nazwaplikuRESX.Replace(".resx", ".KeyValue.json");

            nazwadocelowegoplikuJSON = nazwaplikuRESX.Replace(".resx", ".json");

            if (File.Exists(nazwadocelowegoplikuJSON) == true) { File.Delete(nazwadocelowegoplikuJSON); }


            if (File.Exists(nazwaplikuKeyValueJSON))
            {
                bool wykryto_blad = false;

                List<string> lista_wykrytych_STRINGS = new List<string>();
                List<string> lista_wykrytych_VALUES = new List<string>();

                uint plik_KeyValueJSON_liczbalinii = PoliczLiczbeLinii(nazwaplikuKeyValueJSON);

                UtworzNaglowekJSON(nazwadocelowegoplikuJSON);

                FileStream plik_KeyValueJSON_fs = new FileStream(nazwaplikuKeyValueJSON, FileMode.Open, FileAccess.Read);
                FileStream plik_docelowyJSON_fs = new FileStream(nazwadocelowegoplikuJSON, FileMode.Append, FileAccess.Write);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VALUES = 0;

                    StreamReader plik_KeyValueJSON_sr = new StreamReader(plik_KeyValueJSON_fs);
                    StreamWriter plik_docelowyJSON_sw = new StreamWriter(plik_docelowyJSON_fs);

                    int plik_KeyValueJSON_linia = 1;
                    while (plik_KeyValueJSON_sr.Peek() != -1)
                    {
                        string tresc_linii_KeyValueJSON = plik_KeyValueJSON_sr.ReadLine();


                        if (tresc_linii_KeyValueJSON.Contains("\"Key\": "))
                        {
                            string tresc_KEY = tresc_linii_KeyValueJSON.TrimStart().Split(new char[] { '\"' })[3];

                            lista_wykrytych_VALUES.Add(tresc_KEY);

                            //Console.WriteLine("Linia nr." + plik_KeyValueJSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                            ilosc_wykrytych_VALUES++;

                        }
                        else if (tresc_linii_KeyValueJSON.Contains("\"Value\": "))
                        {

                            string tresc_STRING = tresc_linii_KeyValueJSON.TrimStart()

                                .Replace("\n", "\\n")
                                .Replace("\\\"", "<bs_n1>")

                                .Split(new char[] { '\"' })[3]

                                .Replace("<bs_n1>", "\\\"");

                            if (tresc_STRING == "")
                            {
                                tresc_STRING = " ";
                            }

                            lista_wykrytych_STRINGS.Add(tresc_STRING);

                            //Console.WriteLine("Linia nr." + plik_KeyValueJSON_linia + " konwersja stringa o treści: " + tresc_STRING);

                            ilosc_wykrytych_STRINGS++;

                        }


                        Console.WriteLine("Trwa konwertowanie linii nr. " + plik_KeyValueJSON_linia + "/" + plik_KeyValueJSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_KeyValueJSON_linia, plik_KeyValueJSON_liczbalinii) + "%]");

                        plik_KeyValueJSON_linia++;
                    }


                    if (lista_wykrytych_VALUES.Count() == lista_wykrytych_STRINGS.Count())
                    {
                        for (int l = 0; l < lista_wykrytych_VALUES.Count(); l++)
                        {
                            string _KLUCZ = lista_wykrytych_VALUES[l];
                            string _STRING = lista_wykrytych_STRINGS[l];

                            plik_docelowyJSON_sw.Write("    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                            if (l + 1 != lista_wykrytych_VALUES.Count())
                            {
                                plik_docelowyJSON_sw.Write(",\n");
                            }
                            else
                            {
                                plik_docelowyJSON_sw.Write("\n");
                            }
                        }
                    }
                    else
                    {
                        wykryto_blad = true;

                        Blad("BŁĄD: Ilość wykrytych VALUES i STRINGS w pliku \"" + nazwaplikuKeyValueJSON + "\" nie zgadza się.");
                    }


                    plik_KeyValueJSON_sr.Close();
                    plik_docelowyJSON_sw.Close();

                    UtworzStopkeJSON(nazwadocelowegoplikuJSON);



                }
                catch
                {
                    wykryto_blad = true;

                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                plik_KeyValueJSON_fs.Close();
                plik_docelowyJSON_fs.Close();


                if (wykryto_blad == false)
                {
                    Sukces("Plik docelowy JSON o nazwie \"" + nazwadocelowegoplikuJSON + "\" został utworzony.");
                }
                else
                {
                    if (File.Exists(nazwadocelowegoplikuJSON) == true) { File.Delete(nazwaplikuKeyValueJSON); }
                }


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }


        }

        public static void JSONtoRESX()
        {
            bool wykryto_blad = false;

            string nazwaplikuJSON;
            string nazwanowegoplikuRESX;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "MyTexts.pl-PL.json"; }

            nazwanowegoplikuRESX = "NOWY_" + nazwaplikuJSON.Replace(".json", ".resx");

            if (File.Exists(nazwaplikuJSON) == true)
            {

                dynamic[] _plikJSON_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(nazwaplikuJSON);
                List<dynamic> _plikJSON_listakluczy = _plikJSON_tablicalistdanych[0];
                List<List<dynamic>> _plikJSON_listastringow = _plikJSON_tablicalistdanych[1];

                //Console.WriteLine("[DEBUG] _plikJSON_listakluczy[5]=" + _plikJSON_listakluczy[5]);
                //Console.WriteLine("[DEBUG] _plikJSON_listastringow[5][0]=" + _plikJSON_listastringow[5][0]);


                if (File.Exists(nazwanowegoplikuRESX) == true) { File.Delete(nazwanowegoplikuRESX); }

                UtworzNaglowekRESX(nazwanowegoplikuRESX);

                FileStream nowyplikRESX_fs = new FileStream(nazwanowegoplikuRESX, FileMode.Append, FileAccess.Write);

                try
                {
                    StreamWriter nowyplikRES_sw = new StreamWriter(nowyplikRESX_fs);

                    for (int op1 = 0; op1 < _plikJSON_listakluczy.Count; op1++)
                    {
                        if (_plikJSON_listakluczy[op1] != "$id" && _plikJSON_listakluczy[op1] != "strings")
                        {
                            string _KLUCZ = _plikJSON_listakluczy[op1];
                            string _STRING = FiltrujStringDoZapisuWRESX(_plikJSON_listastringow[op1][0]);

                            nowyplikRES_sw.WriteLine("  <data name=\"" + _KLUCZ + "\" xml:space=\"preserve\">");
                            nowyplikRES_sw.WriteLine("    <value>" + _STRING + "</value>");
                            nowyplikRES_sw.WriteLine("  </data>");
                        }

                    }

                    nowyplikRES_sw.Close();

                }
                catch (Exception blad)
                {
                    Blad("Błąd: Wystąpił nieoczekiwany wyjątek. (" + blad + ")");
                }


                nowyplikRESX_fs.Close();

                UtworzStopkeRESX(nazwanowegoplikuRESX);


            }
            else
            {
                Blad("Błąd: Nie istnieje plik o nazwie \"" + nazwaplikuJSON + "\".");
            }


            if (wykryto_blad == false)
            {
                Sukces("Plik RESX o nazwie \"" + nazwanowegoplikuRESX + "\" został utworzony.");
            }
            else
            {
                if (File.Exists(nazwanowegoplikuRESX) == true) { File.Delete(nazwanowegoplikuRESX); }
            }


        }

        public static void JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON_v2()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(nazwaplikuJSON))
            {
                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_transifexCOMkeystxt_fs = new FileStream(nazwaplikuJSON + ".keysTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaplikuJSON + ".stringsTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {

                    dynamic[] danezplikuJSON_tablicalistdanych = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1(nazwaplikuJSON);
                    List<dynamic> danezplikuJSON_listakluczy = danezplikuJSON_tablicalistdanych[0];
                    List<List<dynamic>> danezplikuJSON_listastringow = danezplikuJSON_tablicalistdanych[1];

                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_transifexCOMkeystxt_sw = new StreamWriter(nowy_plik_transifexCOMkeystxt_fs);
                    StreamWriter nowy_plik_transifexCOMstringstxt_sw = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                    int _ID = 4;
                    for (int iv1 = 0; iv1 < danezplikuJSON_listakluczy.Count; iv1++)
                    {
                        int aktualnynumerlinii = iv1 + 1;

                        for (int iv2 = 0; iv2 < danezplikuJSON_listastringow[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("danezplikuJSON_listastringow[" + iv1 + "][" + iv2 + "]: " + danezplikuJSON_listastringow[iv1][iv2]);

                            if (danezplikuJSON_listakluczy[iv1] != "$id" && danezplikuJSON_listastringow[iv1][iv2] != "1")
                            {
                                string _KLUCZ = FiltrujString(danezplikuJSON_listakluczy[iv1]);
                                string _STRING = FiltrujString(danezplikuJSON_listastringow[iv1][iv2])
                                                 .Replace("\\n", "<br>")
                                                 .Replace("\\\"", "<bs_n1>")
                                                 .Replace("\\\\", "/")
                                                 ;

                                nowy_plik_transifexCOMkeystxt_sw.WriteLine(_KLUCZ);
                                nowy_plik_transifexCOMstringstxt_sw.WriteLine("<" + _ID + ">" + _STRING);

                                _ID++;
                            }

                        }

                        Console.WriteLine("Trwa konwertowanie linii nr. " + aktualnynumerlinii + "/" + danezplikuJSON_listakluczy.Count + " [" + PoliczPostepWProcentach(aktualnynumerlinii, danezplikuJSON_listakluczy.Count) + "%]");

                    }





                    plik_JSON_sr.Close();
                    nowy_plik_transifexCOMkeystxt_sw.Close();
                    nowy_plik_transifexCOMstringstxt_sw.Close();

                    Sukces("Utworzono 2 pliki:");
                    Sukces("-\"" + nazwaplikuJSON + ".keysTransifexCOM.txt\": przeznaczony dla narzędzia PWRlangConverter.");
                    Sukces("-\"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\": przeznaczony dla platformy Transifex.");
                    

                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                plik_JSON_fs.Close();
                nowy_plik_transifexCOMkeystxt_fs.Close();
                nowy_plik_transifexCOMstringstxt_fs.Close();


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

            if (File.Exists(nazwaplikuJSON + ".keys.txt") && File.Exists(nazwaplikuJSON + ".strings.txt"))
            {
                Console.WriteLine("----------------------------------");
                Sukces("Utworzono 2 pliki TXT: \"" + nazwaplikuJSON + ".keysTransifexCOM.txt\" oraz \"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\"");

            }





        }

        public static void JSONplusJSONtoJSON_PrzeniesienieStringowWedlugSzablonu_v2()
        {
            Console.Write("Podaj nazwę źródłowego pliku JSON, z którego ma zostać przeniesiona treść stringów: ");
            string plikJSONzrodlowy_nazwa = Console.ReadLine();
            if (plikJSONzrodlowy_nazwa == "") { plikJSONzrodlowy_nazwa = "test1.json"; }

            Console.Write("Podaj nazwę szablonowego pliku JSON, według którego ma zostać utworzony nowy plik: ");
            string plikJSONszablonowy_nazwa = Console.ReadLine();
            if (plikJSONszablonowy_nazwa == "") { plikJSONszablonowy_nazwa = "test2.json"; }

            string plikJSONdocelowy_nazwa = "NOWY_" + plikJSONszablonowy_nazwa;
            if (File.Exists(plikJSONdocelowy_nazwa)) { File.Delete(plikJSONdocelowy_nazwa); }

            if (File.Exists(plikJSONzrodlowy_nazwa) && File.Exists(plikJSONszablonowy_nazwa))
            {
                List<RekordJSON> plikJSONzrodlowy_listarekordow = WczytajDaneZPlikuJSONdoListyRekordow(plikJSONzrodlowy_nazwa);
                List<RekordJSON> plikJSONszablonowy_listarekordow = WczytajDaneZPlikuJSONdoListyRekordow(plikJSONszablonowy_nazwa);
                List<RekordJSON> plikJSONdocelowy_listarekordow = new List<RekordJSON>();

                //int aktualnyID_dlaplikudocelowegoJSON = 0 + 2;

                for (int wr = 0; wr < plikJSONszablonowy_listarekordow.Count(); wr++)
                {
                    List<RekordJSON> lista_znalezioneklucze_wplikuJSONzrodlowym = plikJSONzrodlowy_listarekordow.FindAll(x => x.Klucz == plikJSONszablonowy_listarekordow[wr].Klucz);

                    if (lista_znalezioneklucze_wplikuJSONzrodlowym.Count() == 1)
                    {

                        //Console.WriteLine("[DEBUG] Znaleziono klucz: " + lista_znalezioneklucze_wplikuJSONzrodlowym[0].Klucz);

                        plikJSONdocelowy_listarekordow.Add(new RekordJSON { ID = plikJSONszablonowy_listarekordow[wr].ID, Plik = plikJSONdocelowy_nazwa, Klucz = lista_znalezioneklucze_wplikuJSONzrodlowym[0].Klucz, String = lista_znalezioneklucze_wplikuJSONzrodlowym[0].String });

                        //aktualnyID_dlaplikudocelowegoJSON++;

                    }
                    else if (lista_znalezioneklucze_wplikuJSONzrodlowym.Count() == 0)
                    {

                        plikJSONdocelowy_listarekordow.Add(new RekordJSON { ID = plikJSONszablonowy_listarekordow[wr].ID, Plik = plikJSONdocelowy_nazwa, Klucz = plikJSONszablonowy_listarekordow[wr].Klucz, String = plikJSONszablonowy_listarekordow[wr].String });
                    
                    }
                    else
                    {
                        Blad("Krytyczny błąd: W pliku źródłowym JSON występuje więcej niż 1 string zawierający ten sam klucz o wartości: \"" + lista_znalezioneklucze_wplikuJSONzrodlowym[wr].Klucz + "\".");
                    }

                }

                bool czy_utworzono_naglowek = UtworzNaglowekJSON(plikJSONdocelowy_nazwa);

                if (czy_utworzono_naglowek == true)
                {
                    FileStream plikJSONdocelowy_fs = new FileStream(plikJSONdocelowy_nazwa, FileMode.Append, FileAccess.Write);

                    try
                    {
                        StreamWriter plikJSONdocelowy_sw = new StreamWriter(plikJSONdocelowy_fs);

                        for (int zd = 0; zd < plikJSONdocelowy_listarekordow.Count(); zd++)
                        {

                            string _KLUCZ = plikJSONdocelowy_listarekordow[zd].Klucz;
                            string _STRING = plikJSONdocelowy_listarekordow[zd].String;

                            plikJSONdocelowy_sw.Write(/*"[DEBUG-ID: " + plikJSONdocelowy_listarekordow[zd].ID + "] " + */"    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                            if (zd + 1 != plikJSONdocelowy_listarekordow.Count())
                            {
                                plikJSONdocelowy_sw.Write(",");
                            }

                            plikJSONdocelowy_sw.Write("\n");

                        }

                        plikJSONdocelowy_sw.Close();
                    }
                    catch
                    {
                        Blad("Wystąpił problem z zapisem do nowogenerowanego pliku JSON: " + plikJSONdocelowy_nazwa);
                    }

                    plikJSONdocelowy_fs.Close();

                    bool czy_utworzono_stopke = UtworzStopkeJSON(plikJSONdocelowy_nazwa);

                    if (czy_utworzono_stopke == true)
                    {
                        Sukces("Plik JSON o nazwie \"" + plikJSONdocelowy_nazwa + "\" został wygenerowany.");
                    }
                    else
                    {
                        Blad("BŁĄD: Wystąpił problem z utworzeniem stopki w nowogenerowanym pliku JSON: " + plikJSONdocelowy_nazwa);
                    }

                }
                else
                {
                    Blad("BŁĄD: Wystąpił problem z utworzeniem nagłówka w nowogenerowanym pliku JSON: " + plikJSONdocelowy_nazwa);
                }

            }
            else
            {
                Blad("BŁĄD: Nie istnieje przynajmniej jeden ze wskazanych plików.");
            }

        }


    }

}