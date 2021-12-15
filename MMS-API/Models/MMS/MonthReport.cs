using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.MMS
{
    public class MonthReport
    {
        [Key]
        public int StockId { get; set; }
        public int Revenue { get; set; }
        [Key]
        public string YearMonth { get; set; }
        public string YearQ { get; set; }

        public int PreRevenue { get; set; }
        public DateTime UpdateTime {get;set;}

    }
}
