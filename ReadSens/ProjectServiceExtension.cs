using Scada.AddIn.Contracts;
using System.IO;
using System.Net;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nancy.Json;
using System.Text;
using System.Collections;

namespace ReadSens
{
    /// <summary>
    /// Description of Project Service Extension.
    /// </summary>
    [AddInExtension("Your Project Service Extension Name", "Your Project Service Extension Description", DefaultStartMode = DefaultStartupModes.Auto)]
    public class ProjectServiceExtension : IProjectServiceExtension
    {
        #region IProjectServiceExtension implementation
        Scada.AddIn.Contracts.IProject project;


        public void WriteValue(string name, string value)
        {
            project.VariableCollection[name].SetValue(0, value);
        }
        public object GetValue(string name)
        {
            return project.VariableCollection[name].GetValue(0);
        }


        public string ServerResponse(string query)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mehatronika.ddns.net:40001/iot/iotstanjemodula");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new

            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    idmodula = query
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        public string SetIzlaz0OnCloud(string query, byte q_izlaz0)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mehatronika.ddns.net:40001/iot/iotstanjemodula");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new

            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    idmodula = query,
                    izlaz0 = q_izlaz0
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        public string SetIzlaz1OnCloud(string query, byte q_izlaz1)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mehatronika.ddns.net:40001/iot/iotstanjemodula");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new

            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    idmodula = query,
                    izlaz1 = q_izlaz1
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        public string OutputHistory(string query, string startDate, string endDate)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mehatronika.ddns.net:40001/iot/istorijaizlaz");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new

            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    idmodula = query,
                    start = startDate,
                    end = endDate
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        public string InputHistory(string query, string startDate, string endDate)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://mehatronika.ddns.net:40001/iot/istorijaulaz");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new

            StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    idmodula = query,
                    start = startDate,
                    end = endDate
                });

                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }


        static byte[] ValueToBits(byte input)
        {
            BitArray ba = new BitArray(new byte[] { input });
            byte[] number = new byte[8];

            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                {
                    number[7 - i] = 1;
                }
                else
                {
                    number[7 - i] = 0;
                }
            }
            return number;
        }
        static byte[] Masking(byte[] inputArray, byte[] mask)
        {
            byte[] izlaz = new byte[8];

            for (int i = 0; i < izlaz.Length; i++)
            {
                izlaz[i] = (byte)(inputArray[i] & mask[i]);
            }

            return izlaz;
        }
        static byte ValueFromBits(byte[] array)
        {
            byte izlaz = 0;
            byte maxBit = 128;

            for (int i = 0; i < array.Length; i++)
            {

                if (array[i] == 1)
                {
                    izlaz += (byte)(maxBit / Math.Pow(2, i));
                }
            }

            return izlaz;
        }


        class Parametar {
            public string idmodula { get; set; }
            public byte ulaz0 { get; set; }
            public byte ulaz1 { get; set; }
            public byte izlaz0 { get; set; }
            public byte izlaz1 { get; set; }
            public string time { get; set; }
        };


        public void Start(IProject context, IBehavior behavior)
        {

            project = context;

            //maske
            byte[] maskaRadStanice = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 };
            byte[] maskaVakum = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 };
            byte[] maskaCilindarMagacina = new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 };
            byte[] maskaPrazanMagacin = new byte[] { 0, 0, 1, 0, 0, 0, 0, 0 };
            byte[] maskaCilindarUvucen = new byte[] { 0, 0, 0, 0, 0, 0, 1, 0 };
            byte[] maskaKran = new byte[] { 1, 1, 0, 0, 0, 0, 0, 0 };
            byte[] maskaHvataljka_i_Lift = new byte[] { 0, 0, 0, 0, 1, 1, 0, 0 };
            byte[] maskaTraka = new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 };

            string json = ServerResponse("");
            List<Parametar> prviIO = new List<Parametar>();
            prviIO = JsonConvert.DeserializeObject<List<Parametar>>(json);


            //********************** FLAGS ******************************//
            bool flag_radStanice1 = false, flag_radStanice2 = false, flag_radStanice3 = false, flag_radStanice4 = false;
            bool flag_vakum1 = false, flag_vakum2 = false;
            bool flag_traka = false, flag_hvataljka = false;

            //**********************   STANICA 1   **********************//
            byte[] bit_PrviModul_Izlaz0 = ValueToBits(prviIO[0].izlaz0);


            byte[] maskiranRadStanice1 = Masking(bit_PrviModul_Izlaz0, maskaRadStanice);
            byte vrednostRadStanice1 = ValueFromBits(maskiranRadStanice1);
            if (vrednostRadStanice1 == 1 && flag_radStanice1 == true)
            {
                flag_radStanice1 = false;
                WriteValue("var1", "radi");

                int cnt = 0;
                cnt++;
                WriteValue("stringara", cnt.ToString());
            }
            else
            {
                flag_radStanice1 = true;
                WriteValue("var1", "stoji");

                WriteValue("stringara", flag_radStanice1.ToString());
            }

            byte[] maskiranVakum1 = Masking(bit_PrviModul_Izlaz0, maskaVakum);
            byte vrednostVakum1 = ValueFromBits(maskiranVakum1);
            if(vrednostVakum1 == 128 && flag_vakum1 == true)
            {
                flag_vakum1 = false;
                //WriteValue("testVar", "1");
            }
            else
            {
                flag_vakum1 = true;
                //WriteValue("testVar", "0");
            }

            byte[] maskiranCilindarMagacina = Masking(bit_PrviModul_Izlaz0, maskaCilindarMagacina);
            byte vrednostCilindarMagacina = ValueFromBits(maskiranCilindarMagacina);
            if (vrednostCilindarMagacina == 8)
            {
                //WriteValue("", "1");
            }
            else
            {
                //WriteValue("", "0");
            }

            byte[] maskiranPrazanMagacin = Masking(bit_PrviModul_Izlaz0, maskaPrazanMagacin);
            byte vrednostPrazanMagacin = ValueFromBits(maskiranPrazanMagacin);
            if (vrednostPrazanMagacin == 32)
            {
                //WriteValue("", "1");
            }
            else
            {
                //WriteValue("", "0");
            }


            //**********************   STANICA 2   **********************//
            byte[] bit_PrviModul_Izlaz1 = ValueToBits(prviIO[0].izlaz1);


            byte[] maskiranRadStanice2 = Masking(bit_PrviModul_Izlaz1, maskaRadStanice);
            byte vrednostRadStanice2 = ValueFromBits(maskiranRadStanice2);
            if (vrednostRadStanice2 == 1 && flag_radStanice2 == true)
            {
                flag_radStanice2 = false;
                //WriteValue("", "1");
            }
            else
            {
                flag_radStanice2 = true;
                //WriteValue("", "0");
            }

            byte[] maskiranVakum2 = Masking(bit_PrviModul_Izlaz1, maskaVakum);
            byte vrednostVakum2 = ValueFromBits(maskiranVakum2);
            if (vrednostVakum2 == 128 && flag_vakum2 == true)
            {
                flag_vakum2 = false;
                //WriteValue("", "1");
            }
            else
            {
                flag_vakum1 = true;
                //WriteValue("", "0");
            }

            byte[] maskiranLift = Masking(bit_PrviModul_Izlaz1, maskaHvataljka_i_Lift);
            byte vrednostLift = ValueFromBits(maskiranLift);
            if (vrednostLift == 12)
            {
                //WriteValue("", "1");
            }
            else
            {
                //WriteValue("", "0");
            }

            byte[] maskiranCilindarUvucen = Masking(bit_PrviModul_Izlaz1, maskaCilindarUvucen);
            byte vrednostCilindarUvucen = ValueFromBits(maskiranCilindarUvucen);
            if (vrednostCilindarUvucen == 2)
            {
                //WriteValue("", "1");
            }
            else
            {
                //WriteValue("", "0");
            }


            //**********************   STANICA 3   **********************//
            byte[] bit_DrugiModul_Izlaz0 = ValueToBits(prviIO[1].izlaz0);


            byte[] maskiranRadStanice3 = Masking(bit_DrugiModul_Izlaz0, maskaRadStanice);
            byte vrednostRadStanice3 = ValueFromBits(maskiranRadStanice3);
            if (vrednostRadStanice3 == 1 && flag_radStanice3 == true)
            {
                flag_radStanice3 = false;
                //WriteValue("", "1");
            }
            else
            {
                flag_radStanice3 = true;
                //WriteValue("", "0");
            }

            byte[] maskiranKran = Masking(bit_DrugiModul_Izlaz0, maskaKran);
            byte vrednostKran = ValueFromBits(maskiranKran);
            if (vrednostKran == 192)
            {
                //WriteValue("testVar", "1");
            }
            else
            {
               //WriteValue("testVar", "0");
            }

            byte[] maskiranHvataljka = Masking(bit_DrugiModul_Izlaz0, maskaHvataljka_i_Lift);
            byte vrednostHvataljka = ValueFromBits(maskiranHvataljka);
            if (vrednostHvataljka == 12 && flag_hvataljka == true)
            {
                flag_hvataljka = false;
               //WriteValue("", "1");
            }
            else
            {
                flag_hvataljka = true;
                //WriteValue("", "0");
            }


            //**********************   STANICA 4   **********************//
            byte[] bit_DrugiModul_Izlaz1 = ValueToBits(prviIO[1].izlaz1);


            byte[] maskiranRadStanice4 = Masking(bit_DrugiModul_Izlaz1, maskaRadStanice);
            byte vrednostRadStanice4 = ValueFromBits(maskiranRadStanice4);
            if (vrednostRadStanice4 == 1 && flag_radStanice4 == true)
            {
                flag_radStanice4 = false;
                //WriteValue("", "1");
            }
            else
            {
                flag_radStanice4 = true;
                //WriteValue("", "0");
            }

            byte[] maskiranTraka = Masking(bit_DrugiModul_Izlaz1, maskaTraka);
            byte vrednostTraka = ValueFromBits(maskiranTraka);
            if (vrednostTraka == 16 && flag_traka == true)
            {
                flag_traka = false;
                //WriteValue("testVar", "1");
            }
            else
            {
                flag_traka = true;
                //WriteValue("testVar", "0");
            }

            //***********************  TEST  ********************************//

            /*DateTime trenutnoVreme = DateTime.Now;
                DateTime drugoVreme = trenutnoVreme.AddHours(6);

                TimeSpan oduzeto = drugoVreme.Subtract(trenutnoVreme);
                

                Console.WriteLine(oduzeto.TotalMinutes);
                Console.WriteLine(trenutnoVreme.ToString("yyyy-MM-dd"));*/


            /*byte maskedValue = ValueFromBits(maskiranRadStanice);
            if(maskedValue == 128)
            {
                WriteValue("var1", maskedValue.ToString());
            }*/



            //******************* GET METHOD *************************//

            /*using (var client = new HttpClient())
            {
                var endpoint = new Uri("http://mehatronika.ddns.net:40001/iot/iotstanjemodula");
                var result = client.GetAsync(endpoint).Result;
                string json = result.Content.ReadAsStringAsync().Result;

                List<Parametar> incoming = new List<Parametar>();
                incoming = JsonConvert.DeserializeObject<List<Parametar>>(json);

                //WriteValue("var1", incoming[0].stanje);
            }

            //var ourVariable = GetValue();
            // enter your code which should be executed when starting the SCADA Runtime Service*/
        }
        public void Stop()
        {
            // enter your code which should be executed when stopping the SCADA Runtime Service
        }
        #endregion


    }
}