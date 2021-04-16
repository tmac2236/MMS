
using API.Data.Interface.CMS;
using API.Models.CMS;

namespace API.Data.Repository.CMS
{
    public class CMSCarManageRecordDAO: CMSCommonDAO<CarManageRecord>, ICMSCarManageRecordDAO
    {
        public CMSCarManageRecordDAO(CMSContext context) : base(context)
        {
        }

    }
}