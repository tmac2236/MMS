using System.Collections.Generic;
using System.Linq;
using API.Data.Interface.CMS;
using API.Helpers;
using API.Models.CMS;
using CMS_API.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace API.Data.Repository.CMS
{
    public class CMSCarManageRecordDAO : CMSCommonDAO<CarManageRecord>, ICMSCarManageRecordDAO
    {
        public CMSCarManageRecordDAO(CMSContext context) : base(context)
        {
        }

        public async Task<List<CarManageRecordDto>> GetCarManageRecordDto(SCarManageRecordDto sCarManageRecordDto)
        {

            List<SqlParameter> pc = new List<SqlParameter>{
                new SqlParameter("@LicenseNumber",sCarManageRecordDto.LicenseNumber != null ? sCarManageRecordDto.LicenseNumber.Trim(): (object)DBNull.Value),
                new SqlParameter("@SignInDateS",sCarManageRecordDto.SignInDateS),
                new SqlParameter("@SignInDateE",sCarManageRecordDto.SignInDateE)
            };
            /*
            var data = _context.GetCarManageRecordDto
                   .FromSqlRaw("EXECUTE dbo.CMS_GetCarManageRecordDto @LicenseNumber,@SignInDateS,@SignInDateE", pc.ToArray())
                   .ToList();
            */
            string strWhere = "";
            if (sCarManageRecordDto.LicenseNumber == "" || sCarManageRecordDto.LicenseNumber == null)
                strWhere += " WHERE SignInDate between @SignInDateS and @SignInDateE";
            else
                strWhere += " WHERE SignInDate between @SignInDateS and @SignInDateE AND @LicenseNumber = LicenseNumber ";

            string strSQL = string.Format(@"
                                            SELECT 
                                            	   CP.CompanyName	  AS	CompanyName
                                                  ,PlateNumber		  AS	PlateNumber
                                                  ,DriverName		  AS	DriverName
                                                  ,LicenseNumber	  AS	LicenseNumber
                                                  ,SignInDate		  AS	SignInDate
                                            
                                                  ,TempNumber		  AS	TempNumber
                                                  ,SignInReason		  AS	SignInReason
                                                  ,GoodsName		  AS	GoodsName
                                                  ,GoodsCount		  AS	GoodsCount
                                                  ,DPM.DepartmentName AS 	DepartmentName
                                            
                                                  ,ContactPerson	  AS	ContactPerson
                                                  ,SealNumber		  AS	SealNumber
                                                  ,DriverSign		  AS	DriverSign
                                                  ,SignOutDate		  AS	SignOutDate
                                                  ,GuardName	      AS	GuardName
                                            
                                                  ,C.CarSize		  AS	CarSize
                                            	  ,CP.CompanyDistance AS	CompanyDistance
                                                  ,0                AS    isDisplay
                                            
                                              FROM CMSCarManageRecord AS	CMR
                                              left join CMSCompany AS CP on CP.Id = CMR.CompanyId
                                              left join CMSDepartment AS DPM on DPM.ID = CMR.DepartmentId
                                              left join CMSCar AS C on C.Id = CMR.CarId ");
            strSQL += strWhere;                                  
            var data = await _context.GetCarManageRecordDto.FromSqlRaw(strSQL, pc.ToArray()).ToListAsync();

            return data;

        }
    }
}