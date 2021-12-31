
using System.Collections.Generic;
using System.Linq;
using API.Data.Interface.MMS;
using API.Models.MMS;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository.MMS
{
    public class QuarterReportDAO: MMSCommonDAO<QuarterReport>, IQuarterReportDAO
    {
        public QuarterReportDAO(MMSContext context) : base(context)
        {
        }

        public List<QuarterReport> GetTop4Eps()
        {

            string strSQL = string.Format(@";WITH top4 AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY StockId ORDER BY YearQ DESC) AS rn
   FROM QuarterReport
)
SELECT StockId,Eps,YearQ,PreEps,UpdateTime,TheEps
FROM top4
WHERE rn < 5");

            var data = _context.QuarterReport
            .FromSqlRaw(strSQL).ToList();
            return data;
        }
    }
}