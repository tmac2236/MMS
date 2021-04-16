
using API.Data.Interface.CMS;
using API.Models.CMS;

namespace API.Data.Repository.CMS
{
    public class CMSCompanyDAO: CMSCommonDAO<Company>, ICMSCompanyDAO
    {
        public CMSCompanyDAO(CMSContext context) : base(context)
        {
        }

    }
}