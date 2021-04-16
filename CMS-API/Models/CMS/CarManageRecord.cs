using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Models.CMS
{
    public class CarManageRecord
    {
        [Key]
        public string Id { get; set; }
        public int CompanyId { get; set; }
        public string PlateNumber { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public System.DateTime SignInDate { get; set; }
        public string TempNumber { get; set; }
        public string SignInReason { get; set; }
        public string GoodsName { get; set; }
        public string GoodsCount { get; set; }
        public int DepartmentId { get; set; }
        public string ContactPerson { get; set; }
        public string SealNumber { get; set; }
        public string DriverSign { get; set; }
        public System.DateTime SignOutDate { get; set; }
        public string GuardName { get; set; }
    }
}
