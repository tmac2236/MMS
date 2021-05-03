using API.Models.MMS;
using API.Data.Interface.MMS;

namespace API.Data.Repository.MMS
{
    public class CMSDepartmentDAO: CMSCommonDAO<Department>, ICMSDepartmentDAO
    {
        public CMSDepartmentDAO(CMSContext context) : base(context)
        {
        }

    }
}