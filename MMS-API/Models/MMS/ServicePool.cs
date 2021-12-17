using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.MMS
{
    public class ServicePool
    {
        [Key]
        public string SerName { get; set; }
        [Key]
        public string Param { get; set; }

        public DateTime OccTime { get; set; }
        public string Type { get; set; }
        public string Emessage { get; set; }
        public int Code { get; set; }

    }
}
