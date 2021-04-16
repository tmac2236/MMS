
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.CMS
{
    public class Company
    {
        [Key]
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public decimal CompanyDistance { get; set; }
    }
}
