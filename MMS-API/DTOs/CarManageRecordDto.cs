using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMS_API.DTOs
{
    public class CarManageRecordDto
    {
        public string CompanyName { get; set; }
        public string PlateNumber { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime SignInDate { get; set; }


        public string TempNumber { get; set; }
        public string SignInReason { get; set; }
        public string GoodsName { get; set; }
        public string GoodsCount { get; set; }
        public string DepartmentName { get; set; }

        public string ContactPerson { get; set; }
        public string SealNumber { get; set; }
        public string DriverSign { get; set; }
        public DateTime? SignOutDate { get; set; }
        public string GuardName { get; set; }

        public string CarSize { get; set; }
        public string CompanyDistance { get; set; }
        public int isDisplay { get; set; }
    }
}