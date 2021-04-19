using System.Collections.Generic;
using System.Linq;
using API.Data.Interface.CMS;
using API.Helpers;
using API.Models.CMS;
using CMS_API.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;

namespace API.Data.Repository.CMS
{
    public class CMSCarManageRecordDAO : CMSCommonDAO<CarManageRecord>, ICMSCarManageRecordDAO
    {
        public CMSCarManageRecordDAO(CMSContext context) : base(context)
        {
        }

        public PagedList<CarManageRecordDto> GetCarManageRecordDto(SCarManageRecordDto sCarManageRecordDto)
        {
            List<SqlParameter> pc = new List<SqlParameter>{
                new SqlParameter("@LicenseNumber",sCarManageRecordDto.LicenseNumber != null ? sCarManageRecordDto.LicenseNumber.Trim(): (object)DBNull.Value),
                new SqlParameter("@SignInDateS",sCarManageRecordDto.SignInDateS),
                new SqlParameter("@SignInDateE",sCarManageRecordDto.SignInDateE)
            };

            var data = _context.GetCarManageRecordDto
                   .FromSqlRaw("EXECUTE dbo.CMS_GetCarManageRecordDto @LicenseNumber,@SignInDateS,@SignInDateE", pc.ToArray())
                   .ToList();

            return PagedList<CarManageRecordDto>
           .Create(data, sCarManageRecordDto.PageNumber, sCarManageRecordDto.PageSize, sCarManageRecordDto.IsPaging);

        }
    }
}