using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace VuelingAssesment.Controllers
{
    public class HomeController : Controller
    {
        string strLogFile;

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            strLogFile = FileController.createLogFile();

            FileController.getJsonDataFromFile(
                System.Configuration.ConfigurationManager.AppSettings["transactions"], strLogFile);

            FileController.getJsonDataFromFile(
                System.Configuration.ConfigurationManager.AppSettings["rates"], strLogFile);

            return View();
        }

        /**
         * Método que obtiene los usuarios por Post filtrados por
         * la Id del usuario
         * */
        [HttpPost]
        public ActionResult getAllRates()
        {
            string receivedData;
            StringBuilder sb = new StringBuilder();
            try
            {
                var reqStr = System.Configuration.ConfigurationManager.AppSettings["rates"];
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqStr);

                req.ContentType = "application/json";
                var response = (HttpWebResponse)req.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    receivedData = sr.ReadToEnd();
                }

                var data = JsonConvert.DeserializeObject<List<RootObjectRate>>(receivedData);

                foreach (var item in data)
                {
                    sb.Append("From: ");
                    sb.Append(item.from);
                    sb.Append(" ");
                    sb.Append("To: ");
                    sb.Append(item.to);
                    sb.Append(" ");
                    sb.Append("Rate: ");
                    sb.Append(item.rate);
                    sb.Append("\n");

                }
            }
            catch (Exception ex)
            {
                FileController.writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }

            ViewData["results"] = sb.ToString();

            return View("Index");
        }

        [HttpPost]
        public ActionResult getAllTransactions()
        {
            var sb = new StringBuilder();

            try
            {
                var trList = FileController.readDataFromFile(
                System.Configuration.ConfigurationManager.AppSettings["fileTransactions"]);
                foreach (var item in trList)
                {
                    sb.AppendLine(String.Format("SKU: {0}, Amount: {1}, Currency: {2}",
                        item.sku, item.amount, item.currency));
                }
            }
            catch (Exception ex)
            {
                FileController.writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }

            ViewData["results"] = sb.ToString();

            return View("Index");
        }

        [HttpPost]
        public ActionResult getTransactionsBySKU(string sku)
        {
            decimal suma = 0;
            var sbRes = new StringBuilder();
            var sb = new StringBuilder();

            try
            {
                var trList = FileController.readDataFromFile(
                    System.Configuration.ConfigurationManager.AppSettings["fileTransactions"]);
                if (!string.IsNullOrEmpty(sku))
                {
                    foreach (var item in trList)
                    {
                        if (item.sku == sku)
                        {
                            if (!item.currency.Equals("EUR"))
                            {
                                var currencyEUR = CurrencyController.ToEUR(item);
                                sb.AppendLine(item.ToString() + " to EUR: " + Math.Round((Decimal)currencyEUR, 2));
                                suma += currencyEUR;
                            }
                            else if (item.currency.Equals("EUR"))
                            {
                                var amount = decimal.Parse(item.amount,
                                  new NumberFormatInfo() { NumberDecimalSeparator = "." });
                                sb.AppendLine(item.ToString() + " to EUR: " + Math.Round((Decimal)amount, 2));
                                suma += amount;
                            }
                        }
                    }
                }
                else
                {
                    MsgBox(System.Configuration.ConfigurationManager.AppSettings["emptyValueSKU"]);
                }

                
            }
            catch (Exception ex)
            {
                FileController.writeDataIntoLog(ex.StackTrace.ToString(), strLogFile);
            }
            sbRes.AppendLine(suma.ToString("#.##"));
            sbRes.Append(" €");

            ViewData["total"] = sbRes.ToString();
            ViewData["results"] = sb.ToString();
            return View("Index");
        }

        /**
        * Método que imprime por pantalla un mensaje 
        * de una manera más sencilla, utilizando JavaScript
        * Aunque no lo he utilizado suele ser útil
        */
        private void MsgBox(string sMessage)
        {
            string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);
        }
    }
}
