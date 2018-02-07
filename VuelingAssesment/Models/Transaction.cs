using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;


namespace VuelingAssesment
{
    public class RootObjectTransaction
    {
        public string sku { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
    }

    public class Transaction
    {
        public Transaction()
        {

        }

        public Transaction(string _sku, string _amount, string _currency)
        {
            this.sku = _sku;
            this.amount = _amount;
            this.currency = _currency;
        }

        [Required]
        public string sku { get; set; }
        [Required]
        public string amount { get; set; }
        [Required]
        public string currency { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SKU: ");
            sb.Append(sku);
            sb.Append(" Amount: ");
            sb.Append(amount);
            sb.Append(" Currency: ");
            sb.Append(currency);

            return sb.ToString();
        }
    }
}