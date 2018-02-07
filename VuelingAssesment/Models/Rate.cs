using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VuelingAssesment
{
    public class RootObjectRate
    {
        public string from { get; set; }
        public string to { get; set; }
        public string rate { get; set; }
    }

    public class Rate
    {

        public Rate()
        {

        }

        public Rate(string _from, string _to, string _rate)
        {
            this.rate = _from;
            this.to = _to;
            this.rate = _rate;
        }

        [Required]
        public string from { get; set; }
        [Required]
        public string to { get; set; }
        [Required]
        public string rate { get; set; }
    }
}