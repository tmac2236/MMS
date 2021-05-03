using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interface.MMS;
using API.Models.MMS;
using MMS_API.DTOs;

namespace API.Data.Interface.MMS
{
    public interface ICMSCarManageRecordDAO : ICMSCommonDAO<CarManageRecord>
    {
        Task<List<CarManageRecordDto>> GetCarManageRecordDto(SCarManageRecordDto sCarManageRecordDto);
        
    }
}