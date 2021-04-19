using System;
using API.Helpers;

namespace CMS_API.DTOs
{
    public class SCarManageRecordDto : PaginationParams
    {
        public string LicenseNumber { get; set; }
        public string SignInDateS { get; set; }
        public string SignInDateE { get; set; }
    }
}