using API.Models.DKS;
using API.Data.Interface;
using API.Models.CMS;
using API.Data.Interface.CMS;

namespace API.Data.Repository.CMS
{
    public class CMSDepartmentDAO: CMSCommonDAO<Department>, ICMSDepartmentDAO
    {
        public CMSDepartmentDAO(CMSContext context) : base(context)
        {
        }

    }
}