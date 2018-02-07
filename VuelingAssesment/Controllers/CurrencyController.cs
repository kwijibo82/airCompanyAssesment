using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using static VuelingAssesment.Dijkstra.Dijkstra;

namespace VuelingAssesment.Controllers
{
    public class CurrencyController
    {
        internal static decimal ToEUR(Transaction t)
        {
            switch (t.currency)
            {
                case "USD":
                    return calculateCurrency(getPathToConvert('U'), t);
                case "CAD":
                    return calculateCurrency(getPathToConvert('C'), t);
                case "AUD":
                    return calculateCurrency(getPathToConvert('A'), t);

            }

            return 0;
        }

        private static decimal calculateCurrency(List<char> pathCurrencyList, Transaction t)
        {
            var res = decimal.Parse(t.amount, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            var listRate = JsonConvert.DeserializeObject<List<Rate>>(File.ReadAllText(
                System.Configuration.ConfigurationManager.AppSettings["tempDataFolder"] +
                System.Configuration.ConfigurationManager.AppSettings["fileRates"]));

            pathCurrencyList.Reverse();
            foreach (var currencyVertex in pathCurrencyList)
            {
                switch (currencyVertex)
                {
                    case 'A':
                        foreach (var c in listRate)
                        {
                            if (c.to.Equals("AUD"))
                            {
                                res = res * decimal.Parse(c.rate,
                                    new NumberFormatInfo() { NumberDecimalSeparator = "." });
                                break;
                            }
                        }
                        break;
                    case 'C':
                        foreach (var c in listRate)
                        {
                            if (c.to.Equals("CAD"))
                            {
                                res = res * decimal.Parse(c.rate,
                                    new NumberFormatInfo() { NumberDecimalSeparator = "." });
                                break;
                            }
                        }
                        break;
                    case 'E':
                        foreach (var c in listRate)
                        {
                            if (c.to.Equals("EUR"))
                            {
                                res = res * decimal.Parse(c.rate,
                                    new NumberFormatInfo() { NumberDecimalSeparator = "." });
                                break;
                            }
                        }
                        break;
                }
            }

            return res;
        }

        public static List<char> getPathToConvert(char from)
        {
            Graph g = new Graph();
            g.add_vertex('U', new Dictionary<char, int>() { { 'C', 0 }, { 'A', 0 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'U', 0 } });
            g.add_vertex('A', new Dictionary<char, int>() { { 'U', 0 }, { 'E', 0 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'A', 0 } });

            var listShortPath = new List<char>();
            g.shortest_path(from, 'E').ForEach(vertex => listShortPath.Add(vertex));

            return listShortPath;
        }
    }
}