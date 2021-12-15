
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class ServicePoolDAO: MMSCommonDAO<ServicePool>, IServicePoolDAO
    {
        public ServicePoolDAO(MMSContext context) : base(context)
        {
        }

    }
}