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
        };

        public void Start(IProject context, IBehavior behavior)
        {

            project = context;

            string json = ServerResponse("1");
            List<Parametar> incoming = new List<Parametar>();
            incoming = JsonConvert.DeserializeObject<List<Parametar>>(json);

            byte[] bitNumber = ValueToBits(incoming[0].ulaz1);

            byte[] mask = new byte[] { 0, 1, 1, 1, 0, 0, 0, 0};
            byte[] maskedArray = Masking(bitNumber, mask);

            byte maskedValue = ValueFromBits(maskedArray);
            WriteValue("var1", maskedValue.ToString());



            //GET METHOD
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