using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class QuarterReportDto
    {

        public int StockId { get; set; }

        public string YearQ { get; set; }

        public DateTime UpdateTime {get;set;}
        public decimal Eps { get; set; }
        public decimal PreEps { get; set; }
        public decimal TheEps { get; set; }
        

    }
}
