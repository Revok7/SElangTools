﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

namespace PWRlangConverter
{

    public class JSON
    {
        const string skrypt = "JSON.cs";

        //v1 - nie wymaga deklarowania klasy ze schematem danych
        /* zwraca: dynamic[0]=lista stalych (gdzie lista stałych[indeks_stalej]=nazwa stałej) i dynamic[1]=lista wartości (gdzie lista wartości[indeks_stalej][indeks_wartosci]=wartość stałej) */
        public static dynamic[] WczytajStaleIIchWartosciZPlikuJSON_v1(string nazwa_pliku_JSON)
        {
            List<dynamic> lista_stalych = new List<dynamic>();
            List<List<dynamic>> lista_wartosci = new List<List<dynamic>>();
            

            if (File.Exists(nazwa_pliku_JSON) == true)
            {
                ReadOnlySpan<byte> konfiguracja_wczytanyplik = File.ReadAllBytes(nazwa_pliku_JSON);

                var options = new JsonReaderOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                var konfiguracja_dane = new Utf8JsonReader(konfiguracja_wczytanyplik, options);

                string tmp_ostatniodczytanyTokenType = null;
                string tmp_ostatniaodczytanastala = null;
                int tmp_aktualnyindeksstalej = 0;

                while (konfiguracja_dane.Read())
                {

                    if (konfiguracja_dane.TokenType.ToString() != "StartObject" && konfiguracja_dane.TokenType.ToString() != "EndObject")
                    {
                        lista_wartosci.Add(new List<dynamic>());

                        string aktualny_typ_wartosci = konfiguracja_dane.TokenType.ToString();
                        string aktualna_wartosc = konfiguracja_dane.GetString().ToString();

                        // "PropertyName" to stała
                        // "String" to wartość stałej

                        if (tmp_ostatniodczytanyTokenType == null)
                        {
                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] A");

                                //nic nie rób
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] B");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                lista_stalych.Add(aktualna_wartosc);
                            }
                        }
                        else if (tmp_ostatniodczytanyTokenType == "PropertyName")
                        {

                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] C");

                                lista_wartosci[tmp_aktualnyindeksstalej].Add(aktualna_wartosc);
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] D");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                tmp_aktualnyindeksstalej++;
                                lista_stalych.Add(aktualna_wartosc);
                            }

                        }
                        else if (tmp_ostatniodczytanyTokenType == "String")
                        {

                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] E");

                                lista_wartosci[tmp_aktualnyindeksstalej].Add(aktualna_wartosc);
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] F");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                tmp_aktualnyindeksstalej++;
                                lista_stalych.Add(aktualna_wartosc);
                            }

                        }

                        tmp_ostatniodczytanyTokenType = aktualny_typ_wartosci;


                    }


                }

                dynamic[] tablica_list_danych = { lista_stalych, lista_wartosci };

                return tablica_list_danych;


            }
            else
            {
                return null;
            }





            /*
            for (int i0 = 0; i0 < lista_stalych.Count; i0++)
            {
                Console.WriteLine("[DEBUG] lista_stalych[" + i0 + "]: " + lista_stalych[i0]);
            }

            for (int i1 = 0; i1 < lista_wartosci.Count; i1++)
            {
                //Console.WriteLine("lista_wartosci[" + i1 + "]: " + lista_wartosci[i1]);

                for (int i2 = 0; i2 < lista_wartosci[i1].Count; i2++)
                {
                    Console.WriteLine("[DEBUG] lista_wartosci[" + i1 + "][" + i2 + "]: " + lista_wartosci[i1][i2]);
                }


            }
            */







        }

        public static void WyswietlWszystkieStaleIIchWartosci_v1(List<dynamic> lista_stalych, List<List<dynamic>> lista_wartosci)
        {

            for (int i0 = 0; i0 < lista_stalych.Count; i0++)
            {
                Console.WriteLine("[DEBUG] lista_stalych[" + i0 + "]: " + lista_stalych[i0]);
            }

            for (int i1 = 0; i1 < lista_wartosci.Count; i1++)
            {
                //Console.WriteLine("lista_wartosci[" + i1 + "]: " + lista_wartosci[i1]);

                for (int i2 = 0; i2 < lista_wartosci[i1].Count; i2++)
                {
                    Console.WriteLine("[DEBUG] lista_wartosci[" + i1 + "][" + i2 + "]: " + lista_wartosci[i1][i2]);
                }


            }

        }
        /*
        //v2 - wymaga zadeklarowania klasy ze schematem danych
        public static dynamic WczytajStaleIIchWartosciZPlikuJSON_v2(string nazwa_pliku_JSON)
        {
            List<dynamic> lista_stalych = new List<dynamic>();
            List<List<dynamic>> lista_wartosci = new List<List<dynamic>>();

            if (File.Exists(nazwa_pliku_JSON) == true)
            {
                string konfiguracja_wczytanyplik = File.ReadAllText(nazwa_pliku_JSON);
                konfiguracja cfg = JsonSerializer.Deserialize<konfiguracja>(konfiguracja_wczytanyplik);

                return cfg;

            }else
            {
                return null;
            }

        }
        */

    }


}
