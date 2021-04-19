using API.Data.Interface.CMS;
using API.Helpers;
using API.Models.CMS;
using CMS_API.DTOs;

namespace API.Data.Interface.CMS
{
    public interface ICMSCarManageRecordDAO : ICMSCommonDAO<CarManageRecord>
    {
        PagedList<CarManageRecordDto> GetCarManageRecordDto(SCarManageRecordDto sCarManageRecordDto);
    }
}