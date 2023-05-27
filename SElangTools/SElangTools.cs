using System;
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
        readonly static string _SElangTools_naglowek = "SElangTools v.1.02 by Revok (2023)";

        const string skrypt = "SElangTools.cs";
        static public string folderglownyprogramu = Directory.GetCurrentDirectory();

        static DateTime aktualny_czas = DateTime.Now;



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
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("UWAGA!: W celu konwertowania plików pochodzących z platformy Transifex oraz wdrażania aktualizacji (w formacie JSON) należy używać narzędzi PWRlangTools oraz PWRlangConverter z metadanymi dla gry Space Engineers."); Console.ResetColor();
                Console.WriteLine("---[SpaceEngineers_PL]:");
                Console.WriteLine("1. [JSON->RESX] Konwersja pliku lokalizacyjnego JSON do RESX. ");
                Console.WriteLine("-");
                Console.WriteLine("2. [RESX->KeyValueJSON->JSON] Konwersja pliku lokalizacyjnego RESX do JSON.");
                Console.WriteLine("3. [1xJSON->2xTransifex.com.TXT] Konwersja pliku JSON do plików TXT przeznaczonych dla platformy Transifex.com (z identyfikatorami numerów linii według pliku JSON).");
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
                        JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON();
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

        public static void JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "NOWY_MyTexts.pl-PL.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(nazwaplikuJSON))
            {
                uint plik_JSON_liczbalinii = PoliczLiczbeLinii(nazwaplikuJSON);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_transifexCOMkeystxt_fs = new FileStream(nazwaplikuJSON + ".keysTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaplikuJSON + ".stringsTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VARS = 0;
                    List<List<string>> vars_tmp = new List<List<string>>(); //skladnia vars_tmp[numer_linii][0:key||1:ciag_zmiennych]
                    const char separator = ';';


                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_transifexCOMkeystxt_sr = new StreamWriter(nowy_plik_transifexCOMkeystxt_fs);
                    StreamWriter nowy_plik_transifexCOMstringstxt_sr = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                    int plik_JSON_linia = 1;
                    while (plik_JSON_sr.Peek() != -1)
                    {
                        string tresc_linii_JSON = plik_JSON_sr.ReadLine();

                        string tresclinii_ciagzmiennych = "";

                        vars_tmp.Add(new List<string>());


                        string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                        /*
                        for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                        {

                            //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                        if (linia_podzial_1.Length <= 2)
                        {
                            string KEYt1 = linia_podzial_1[0].Trim();
                            int KEYt1_iloscznakow = KEYt1.Length;

                            if (KEYt1_iloscznakow >= 2)
                            {

                                string[] linia_2_separatory = { KEYt1 + "\": \"" };

                                string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                                /*
                                for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                                {

                                    Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                                }
                                */

                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                                if (linia_podzial_2.Length >= 2)
                                {

                                    string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                    int STRINGt1_iloscznakow = STRINGt1.Length;


                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                    if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                    {
                                        string KEY = KEYt1.Remove(0, 1);

                                        int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                        int usunac_znakow = 1;
                                        if (plik_JSON_linia != plik_JSON_liczbalinii - 2)
                                        {
                                            cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                            usunac_znakow = 2;
                                        }

                                        string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                        string STRING = STRINGt2;



                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEY:" + KEY);
                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRING:" + STRING);


                                        if (KEY != "$id")
                                        {

                                            string tresc_KEY = KEY;

                                            try
                                            {
                                                //Console.WriteLine("indeks wykrytego KEY'a: " + ilosc_wykrytych_VARS);

                                                vars_tmp[ilosc_wykrytych_VARS].Add(tresc_KEY);
                                            }
                                            catch
                                            {
                                                Blad("BLAD: vars_tmp #1!");
                                            }

                                            //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                                            ilosc_wykrytych_VARS++;


                                            //string tresc_STRING = STRING;


                                            string tresc_STRING = STRING

                                            .Replace("\\n", "<br>")
                                            .Replace("\\\"", "<bs_n1>")
                                            .Replace("\\\\", "/") // tę linię dodano w PWRlangTools v.1.64 - wykasować ją, jeśli będą występować problemy z parsowaniem pliku JSON wygenerowanego w PWRlangConverter w wersji v.2.03 lub nowszej
                                            ;

                                            if (tresc_STRING == "")
                                            {
                                                tresc_STRING = " ";
                                            }

                                            /*
                                            if (tresc_STRING.Contains('{') || tresc_STRING.Contains('}'))
                                            {
                                                string rodzajenawiasow = "{|}";
                                                int iloscnawiasowwlinii = 0;
                                                Regex regex = new Regex(rodzajenawiasow);
                                                MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                                foreach (var match in matchCollection)
                                                {
                                                    iloscnawiasowwlinii++;
                                                }
                                                if (iloscnawiasowwlinii % 2 == 0)
                                                {
                                                    //Console.WriteLine("Linia nr." + plik_JSON_linia + " posiada pary nawiasów {}.");

                                                    if (tresc_STRING.Contains('{') && tresc_STRING.Contains('}'))
                                                    {
                                                        string[] tresclinii_nawklamrowy_podzial1 = tresc_STRING.Split(new char[] { '{' });

                                                        for (int i1 = 0; i1 < tresclinii_nawklamrowy_podzial1.Length; i1++)
                                                        {
                                                            //Console.WriteLine("tresclinii_nawklamrowy_podzial1[" + i1.ToString() + "]: " + tresclinii_nawklamrowy_podzial1[i1]);

                                                            if (tresclinii_nawklamrowy_podzial1[i1].Contains('}'))
                                                            {
                                                                int kl_index = i1 - 1;
                                                                string tresczwnetrzanawiasuklamrowego = tresclinii_nawklamrowy_podzial1[i1].Split(new char[] { '}' })[0];
                                                                string nazwazmiennej_w_stringstxt = "<kl" + kl_index + ">"; //np. <kl0>, <kl1>, <kl2> itd.

                                                                //Console.WriteLine("tresczwnetrzanawiasuklamrowego (" + i1.ToString() + "): " + tresczwnetrzanawiasuklamrowego);

                                                                tresclinii_ciagzmiennych += "{" + tresczwnetrzanawiasuklamrowego + "}";

                                                                if (i1 + 1 != tresclinii_nawklamrowy_podzial1.Length) { tresclinii_ciagzmiennych += separator; }

                                                                tresc_STRING = tresc_STRING.Replace("{" + tresczwnetrzanawiasuklamrowego + "}", nazwazmiennej_w_stringstxt);

                                                                //Console.WriteLine("nazwazmiennej_w_stringstxt: " + nazwazmiennej_w_stringstxt);
                                                                //Console.WriteLine("tresc_STRING: " + tresc_STRING);


                                                            }


                                                        }

                                                    }



                                                }
                                                else
                                                {
                                                    Blad("BŁĄD: Linia nr." + plik_JSON_linia + " ma błędną ilość nawiasów {}!");
                                                }



                                            }
                                            else
                                            {
                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " NIE posiada pary nawiasów {}.");

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja string'a o tresci: " + tresc_STRING);

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " zawiera VARS: " + tresclinii_ciagzmiennych);

                                            }
                                            */


                                            nowy_plik_transifexCOMstringstxt_sr.WriteLine("<" + plik_JSON_linia + ">" + tresc_STRING);

                                            //vars_tmp[ilosc_wykrytych_STRINGS].Add(tresclinii_ciagzmiennych);

                                            ilosc_wykrytych_STRINGS++;


                                        }

                                    }

                                }

                            }


                        }


                        Console.WriteLine("Trwa konwertowanie linii nr. " + plik_JSON_linia + "/" + plik_JSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_JSON_linia, plik_JSON_liczbalinii) + "%]");

                        plik_JSON_linia++;
                    }

                    //Console.WriteLine("ilosc_wykrytych_vars:" + ilosc_wykrytych_VARS);
                    //Console.WriteLine("vars_tmp[0][0]: " + vars_tmp[0][0]);


                    for (int iv1 = 0; iv1 < vars_tmp.Count; iv1++)
                    {
                        for (int iv2 = 0; iv2 < vars_tmp[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("vars_tmp[" + iv1 + "][" + iv2 + "]: " + vars_tmp[iv1][iv2]);

                            if (iv2 == 0)
                            {
                                nowy_plik_transifexCOMkeystxt_sr.Write(vars_tmp[iv1][iv2]);
                            }
                            else if (iv2 == 1)
                            {
                                if (vars_tmp[iv1][iv2] != "")
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write(separator + vars_tmp[iv1][iv2] + "\n");
                                }
                                else
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write("\n");
                                }
                            }

                            nowy_plik_transifexCOMkeystxt_sr.Write("\n");

                        }
                    }





                    nowy_plik_transifexCOMkeystxt_sr.Close();
                    nowy_plik_transifexCOMstringstxt_sr.Close();
                    plik_JSON_sr.Close();

                    Sukces("Utworzono 2 pliki:");
                    Sukces("-\"" + nazwaplikuJSON + ".keysTransifexCOM.txt\": przeznaczony dla narzędzia PWRlangConverter.");
                    Sukces("-\"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\": przeznaczony dla platformy Transifex.");


                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                nowy_plik_transifexCOMkeystxt_fs.Close();
                nowy_plik_transifexCOMstringstxt_fs.Close();
                plik_JSON_fs.Close();


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


    }
}