
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class MonthReportDAO: MMSCommonDAO<MonthReport>, IMonthReportDAO
    {
        public MonthReportDAO(MMSContext context) : base(context)
        {
        }

    }
}