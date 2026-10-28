using cdn_api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE_033_Rozrachowanie
{
    class Program
    {
        static void Main(string[] args)
        {
             int sesja = 0;
                int XLWersjaAPI = 20220;
                int XLTrybWsadowy = 1;
                int XLWinieta = -1;
                int XLUtworzWlasnaSesje = 1;
                string XLSQLServer = "XL2";
                string XLSQLDatabase = "RTTEST";//"ERPXL_PoloMarket"
                string XLUser = "OS_MP"; //
                string XLPass = "NE!w0rk12!";//"Dupa2003!";

        string connectionString = @"Data Source = XL2; Initial Catalog = ERPXL_RT_Test; User ID = onesupport; Password=g54q@8w#v$Y.Df_8f4hg";


            string dekretyPRZQuery = "SELECT DT_GIDNumer, DT_GIDLp, DT_DC, DT_Kwota, KAZ_NumerDokumentu FROm cdn.Zapisy "
                                    + " join cdn.Zrodla on KAZ_GIDNumer = ZRO_TRNNumer and KAZ_GIDTyp = ZRO_TRNTyp and KAZ_GIDFirma = ZRO_TRNFirma and KAZ_GIDLp = ZRO_TRNLp "
                                    + " join cdn.Dekrety on ZRO_DTNumer = DT_GIDNumer and ZRO_DTTyp = DT_GIDTyp and ZRO_DTLp = DT_GIDLP and ZRO_DTFirma = DT_GIDFirma"
                                    + " WHERE KAZ_NumerDokumentu like 'PRZ %' and KAZ_RP = 2 and KAZ_Anulowany = 0 and DT_DC = 2 and DT_Konto = '190-0000-07-00' and DT_Waluta = 'PLN' and DT_Nierozliczony=1";

            TextWriter tw2 = new StreamWriter(@"C:\onesupport\ONE-033_Rozrachowanie\log.txt", append: true);
            tw2.WriteLine();
            tw2.WriteLine();
            tw2.WriteLine(DateTime.Now.ToString());



            XLLoginInfo_20220 nowyUzytkownik = new XLLoginInfo_20220();
            nowyUzytkownik.Wersja = XLWersjaAPI;
            nowyUzytkownik.Winieta = XLWinieta;
            nowyUzytkownik.TrybWsadowy = XLTrybWsadowy;
            nowyUzytkownik.UtworzWlasnaSesje = XLUtworzWlasnaSesje;
            nowyUzytkownik.SerwerKlucza = XLSQLServer;
            nowyUzytkownik.Baza = XLSQLDatabase;
            nowyUzytkownik.OpeIdent = XLUser;
            nowyUzytkownik.OpeHaslo = XLPass;
            nowyUzytkownik.PortHTTPSerweraKlucza = 1;
            nowyUzytkownik.RodzajSerweraKlucza = 0;
            nowyUzytkownik.SerwerKlucza = @"XL2::5000185869";
            nowyUzytkownik.ProgramID = "ONE_033_Rozrachowanie";

            int blad= cdn_api.cdn_api.XLLogin(nowyUzytkownik, ref sesja);

            if (blad != 0)
            {
                tw2.WriteLine("Błąd logowania API: " + blad);
            }
            else
            {


                List<Dekret> dekretyPRZ = SelectListRowColumn(dekretyPRZQuery, "cdn.Zapisy", connectionString, tw2);
                foreach (Dekret dekretPRZ in dekretyPRZ)
                {
                    tw2.WriteLine("Dekret PRZ: " + dekretPRZ.KAZ_NumerDokumentu);

                    string tresc = dekretPRZ.KAZ_NumerDokumentu.Substring(4);
                    string[] nazwy = tresc.Split('/');

                    string rok = "20" + nazwy[0].Replace("/", "");
                    string seria = nazwy[1].Replace("/", "");
                    string krpNumer = nazwy[2].Replace("/", "");
                    string krpLp = nazwy[3].Replace("/", "");


                    string dekretROZQuery = "SELECT DT_GIDNumer, DT_GIDLp, DT_DC, DT_Kwota, KAZ_NumerDokumentu FROm cdn.Zapisy "
                                            + " join cdn.Zrodla on KAZ_GIDNumer = ZRO_TRNNumer and KAZ_GIDTyp = ZRO_TRNTyp and KAZ_GIDFirma = ZRO_TRNFirma and KAZ_GIDLp = ZRO_TRNLp "
                                            + " join cdn.Dekrety on ZRO_DTNumer = DT_GIDNumer and ZRO_DTTyp = DT_GIDTyp and ZRO_DTLp = DT_GIDLP and ZRO_DTFirma = DT_GIDFirma "
                                            + " join cdn.Raporty on KRP_GIDNumer = KAZ_KRPNumer "
                                            + " WHERE KAZ_Rok = '" + rok + "' and KAZ_Seria = '" + seria + "' and KRP_Numer = " + krpNumer + " and KAZ_KRPLp = " + krpLp + " and DT_Kwota = " + dekretPRZ.DT_Kwota.ToString().Replace(",", ".")
                                            + " and DT_Waluta = 'PLN' and DT_DC = 1 and DT_Konto = '190-0000-07-00'  and DT_Nierozliczony = 1";
                    tw2.WriteLine("Dekret RozQuery: " + dekretROZQuery);
                    List<Dekret> dekretROZ = SelectListRowColumn(dekretROZQuery, "cdn.Zapisy", connectionString, tw2);

                    if (dekretROZ.Count > 0)
                    {
                        tw2.WriteLine("Dekret ROZ: " + dekretROZ[0].DT_GIDNumer);

                        XLRozrachunekInfo_20220 rozrachunek = new XLRozrachunekInfo_20220();
                        rozrachunek.Dekret1DC = dekretPRZ.DT_DC;
                        rozrachunek.Dekret1Lp = dekretPRZ.DT_GIDLp;
                        rozrachunek.Dekret1Numer = dekretPRZ.DT_GIDNumer;
                        rozrachunek.Dekret2DC = dekretROZ[0].DT_DC;
                        rozrachunek.Dekret2Lp = dekretROZ[0].DT_GIDLp;
                        rozrachunek.Dekret2Numer = dekretROZ[0].DT_GIDNumer;
                        rozrachunek.Kwota = dekretPRZ.DT_Kwota;
                        rozrachunek.RozID = 0;
                        rozrachunek.Tryb = 2;
                        rozrachunek.Wersja = XLWersjaAPI;

                        blad = cdn_api.cdn_api.XLRozrachuj(sesja, rozrachunek);
                        if(blad!=0)
                        {
                            tw2.WriteLine("Błąd rozrachowania dekretu "+dekretPRZ.KAZ_NumerDokumentu+" API: " + blad);
                        }


                    }




                }




                cdn_api.cdn_api.XLLogout(sesja);
            }

            tw2.Close();

        }



        public static List<Dekret> SelectListRowColumn(string query, string srcTableName, string connection_string, TextWriter tw2)
        {
            List<string> str = new List<string>();
            List<Dekret> myList = new List<Dekret>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        myList.Add(new Dekret(str[0], str[1], str[2], str[3], str[4]));
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                tw2.WriteLine("Bład SQL:" + ex.ToString());
            }
            return myList;
        }

    }
}
