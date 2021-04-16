using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.CMS
{
    
    public class Department
    {
        [Key]
        public string Id { get; set; }
        public string DepartmentName { get; set; }
    }
}
