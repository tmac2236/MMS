
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class CMSCarDAO: CMSCommonDAO<Car>, ICMSCarDAO
    {
        public CMSCarDAO(CMSContext context) : base(context)
        {
        }

    }
}