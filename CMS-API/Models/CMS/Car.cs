using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Models.CMS
{
    public class Car
    {
        [Key]
        public string Id { get; set; }
        public string CarSize { get; set; }
    }
}
