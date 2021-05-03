
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class CMSCompanyDAO: CMSCommonDAO<Company>, ICMSCompanyDAO
    {
        public CMSCompanyDAO(CMSContext context) : base(context)
        {
        }

    }
}