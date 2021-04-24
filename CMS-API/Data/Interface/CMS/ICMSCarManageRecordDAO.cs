using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interface.CMS;
using API.Models.CMS;
using CMS_API.DTOs;

namespace API.Data.Interface.CMS
{
    public interface ICMSCarManageRecordDAO : ICMSCommonDAO<CarManageRecord>
    {
        Task<List<CarManageRecordDto>> GetCarManageRecordDto(SCarManageRecordDto sCarManageRecordDto);
        
    }
}