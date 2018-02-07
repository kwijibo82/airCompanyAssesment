using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace VuelingAssesment.Controllers
{
    public class FileController
    {
        public static void toFileTransactions(List<RootObjectTransaction> transactionList, string strLogFile)
        {

            if (!Directory.Exists(System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"])) ;
            {
                Directory.CreateDirectory(
                    System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"]);
            }
            try
            {
                var json = JsonConvert.SerializeObject(transactionList);
                File.WriteAllText(
                    System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"] +
                System.Configuration.ConfigurationManager.AppSettings["fileTransactions"], json);
            }
            catch (NullReferenceException ex)
            {
                writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }

        }

        public static void toFileRates(List<RootObjectRate> rateList, string strLogFile)
        {

            if (!Directory.Exists(System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"])) ;
            {
                Directory.CreateDirectory(
                    System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"]);
            }
            try
            {
                var json = JsonConvert.SerializeObject(rateList);
                File.WriteAllText(
                    System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"] +
                System.Configuration.ConfigurationManager.AppSettings["fileRates"], json);
            }
            catch (NullReferenceException ex)
            {
                writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }

        }

        public static void getJsonDataFromFile(string res, string strLogFile)
        {
            string receivedData;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest
                   .Create(res);

                req.ContentType = "application/json";
                var response = (HttpWebResponse)req.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    receivedData = sr.ReadToEnd();
                }

                if (res.Contains("transactions.json"))
                {
                    toFileTransactions(JsonConvert.DeserializeObject<List<RootObjectTransaction>>(receivedData), strLogFile);
                }
                else if (res.Contains("rates.json"))
                {
                    toFileRates(JsonConvert.DeserializeObject<List<RootObjectRate>>(receivedData), strLogFile);
                }
            }
            catch (Exception ex)
            {
                writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }

        }

        public static string createLogFile()
        {
            var folder = System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"] +
                System.Configuration.ConfigurationManager.AppSettings["logFolder"];
            Directory.CreateDirectory(folder);
            string fileStr = folder + $"//log{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt";
            File.Create(fileStr).Close();
            return fileStr;
        }

        public static string writeDataIntoLog(string str, string fileStr)
        {
            FileStream ostrm = null;
            StreamWriter writer = null;
            try
            {
                using (ostrm = new FileStream(fileStr, FileMode.Append, FileAccess.Write))
                {
                    writer = new StreamWriter(ostrm);
                    writer.WriteLine($"[{DateTime.Now.ToLongTimeString()}] --> " + str);
                    writer.Flush();
                    writer.Close();
                }

            }
            catch (Exception ex)
            {
                ex.StackTrace.ToString();
            }
            finally
            {
                ostrm?.Dispose();
            }
            return str;
        }

        public static List<Transaction> readDataFromFile(string file)
        {
            return JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(
                System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"] + file));

        }
    }
}
