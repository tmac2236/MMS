using System.Collections.Generic;
using API.Models.MMS;

namespace API.Data.Interface.MMS
{
    public interface IMonthReportDAO : ICommonDAO<MonthReport>
    {
        public List<MonthReport> GetTop12Month();
    }
}