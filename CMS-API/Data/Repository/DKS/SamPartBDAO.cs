using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using API.Models.DKS;
using API.Data.Repository;
using API.Data.Interface;
using API.DTOs;
using Microsoft.Data.SqlClient;
using System;
using API.Data;

namespace API.Data.Repository
{
    public class SamPartBDAO: DKSCommonDAO<SamPartB>, ISamPartBDAO
    {
        public SamPartBDAO(DKSContext context) : base(context)
        {
        }

    }
}