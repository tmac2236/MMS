
using API.Data.Interface.DKS;
using API.Models.DKS;

namespace API.Data.Repository
{
    public class SamPartBDAO: DKSCommonDAO<SamPartB>, ISamPartBDAO
    {
        public SamPartBDAO(DKSContext context) : base(context)
        {
        }

    }
}