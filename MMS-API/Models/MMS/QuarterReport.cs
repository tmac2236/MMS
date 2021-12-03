using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.MMS
{
    public class QuarterReport
    {
        [Key]
        public int StockId { get; set; }
        [Key]
        public string YearQ { get; set; }

        public DateTime UpdateTime {get;set;}
        public decimal Eps { get; set; }
        public decimal PreEps { get; set; }
        public decimal TheEps { get; set; }

    }
}
