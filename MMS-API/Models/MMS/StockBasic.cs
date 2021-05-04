using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.MMS
{
    public class StockBasic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Family { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? ClosingPrice { get; set; }
    }
}
