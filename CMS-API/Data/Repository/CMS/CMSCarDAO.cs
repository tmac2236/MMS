
using API.Data.Interface.CMS;
using API.Models.CMS;

namespace API.Data.Repository.CMS
{
    public class CMSCarDAO: CMSCommonDAO<Car>, ICMSCarDAO
    {
        public CMSCarDAO(CMSContext context) : base(context)
        {
        }

    }
}