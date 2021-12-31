
using System.Collections.Generic;
using System.Linq;
using API.Data.Interface.MMS;
using API.Models.MMS;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository.MMS
{
    public class MonthReportDAO: MMSCommonDAO<MonthReport>, IMonthReportDAO
    {
        public MonthReportDAO(MMSContext context) : base(context)
        {
        }

        public List<MonthReport> GetTop12Month()
        {

            string strSQL = string.Format(@";WITH top12 AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY StockId ORDER BY YearMonth DESC) AS rn
   FROM MonthReport
)
SELECT StockId,Revenue,YearMonth,YearQ,PreRevenue,UpdateTime
FROM top12
WHERE rn < 13");

            var data = _context.MonthReport
            .FromSqlRaw(strSQL).ToList();
            return data;
        }
    }
}