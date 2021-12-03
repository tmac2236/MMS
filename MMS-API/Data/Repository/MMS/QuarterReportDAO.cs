
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class QuarterReportDAO: MMSCommonDAO<QuarterReport>, IQuarterReportDAO
    {
        public QuarterReportDAO(MMSContext context) : base(context)
        {
        }

    }
}