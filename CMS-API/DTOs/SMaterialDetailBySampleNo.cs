using System;
using API.Helpers;

namespace API.DTOs
{
    public class SF428SampleNoDetail : PaginationParams
    {
        public string SampleNo { get; set; }
        public string MaterialNo { get; set; }
        public string Status { get; set; }
        public string ChkStockNo { get; set; }
    }
}